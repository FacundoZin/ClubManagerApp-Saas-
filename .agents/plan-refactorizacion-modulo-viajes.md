# Plan de Implementación — Refactorización del Módulo de Gestión de Viajes

## Resumen General

Este documento describe los **5 cambios principales** que se deben aplicar al módulo de gestión de viajes del sistema. Los cambios impactan el modelo de datos, la API backend (.NET/EF Core), y el frontend (Vue 3). Se detallan los archivos exactos a modificar, el contenido esperado, y las consideraciones importantes.

---

## Tabla de Archivos Afectados

| Capa | Archivo | Acción |
|------|---------|--------|
| Domain/Models | `APIClub/Domain/ModuloGestionViajes/Models/InscriptoViaje.cs` | **MODIFICAR** |
| Domain/Models | `APIClub/Domain/ModuloGestionViajes/Models/PagoInscriptoViaje.cs` | **NUEVO** |
| Domain/Repositories | `APIClub/Domain/ModuloGestionViajes/Repositories/IViajeReadRepository.cs` | **MODIFICAR** |
| Domain/Repositories | `APIClub/Domain/ModuloGestionViajes/Repositories/IViajeWriteRepository.cs` | **MODIFICAR** |
| Domain/UseCases | `APIClub/Domain/ModuloGestionViajes/useCases/IViajesServices.cs` | **MODIFICAR** |
| Application/DTOs | `APIClub/Application/Dtos/Viajes/Create-Insert/InsertInscriptoViajeDto.cs` | **MODIFICAR** |
| Application/DTOs | `APIClub/Application/Dtos/Viajes/Views/InscriptosDto.cs` | **MODIFICAR** |
| Application/DTOs | `APIClub/Application/Dtos/Viajes/Views/FullViewViajeDto.cs` | **MODIFICAR** |
| Application/DTOs | `APIClub/Application/Dtos/Viajes/Views/PagoInscriptoDto.cs` | **NUEVO** |
| Application/DTOs | `APIClub/Application/Dtos/Viajes/Update/UpdatePagoViajeDto.cs` | **MODIFICAR** |
| Application/Services | `APIClub/Application/Services/ViajesService.cs` | **MODIFICAR** |
| Infrastructure/Data | `APIClub/Infrastructure/Persistence/Data/AppDbcontext.cs` | **MODIFICAR** |
| Infrastructure/Repos | `APIClub/Infrastructure/Persistence/Repositorio/ViajeReadRepository.cs` | **MODIFICAR** |
| Infrastructure/Repos | `APIClub/Infrastructure/Persistence/Repositorio/ViajeWriteRepository.cs` | **MODIFICAR** |
| Controllers | `APIClub/Contrrollers/ViajesController.cs` | **MODIFICAR** |
| Frontend/Services | `frontend/src/services/viajesService.js` | **MODIFICAR** |
| Frontend/Components | `frontend/src/components/ModuloGestionViajes/InscripcionConfirmModal.vue` | **REESCRIBIR** |
| Frontend/Components | `frontend/src/components/ModuloGestionViajes/PagoViajeModal.vue` | **MODIFICAR** |
| Frontend/Views | `frontend/src/views/ModuloGestionViajes/ViajeDetailView.vue` | **MODIFICAR** |
| Migrations | (nueva migración EF Core) | **NUEVO** |

---

## Cambio 1: Desvincular inscriptos de la tabla Socios

### Contexto
Actualmente `InscriptoViaje` tiene una FK `SocioId` que referencia a la tabla `Socios`. Los inscriptos a un viaje **ya no son socios** del club, son personas externas que se cargan al momento de inscribirse. Se necesitan campos propios en la tabla de inscriptos para almacenar los datos de la persona.

### Modelo `InscriptoViaje.cs` — ESTADO ACTUAL:
```csharp
public class InscriptoViaje
{
    public int Id { get; set; }
    public int VarianteViajeId { get; set; }
    public VarianteViaje Variante { get; set; } = null!;
    public int SocioId { get; set; }
    public Socio Socio { get; set; } = null!;
    public decimal montoAbonado { get; set; }
    public decimal MontoPendiente { get; set; }
    public bool cancelado { get; set; } = false;
}
```

### Modelo `InscriptoViaje.cs` — ESTADO DESEADO:
```csharp
namespace APIClub.Domain.ModuloGestionViajes.Models
{
    public class InscriptoViaje
    {
        public int Id { get; set; }

        // Datos personales del inscripto (independientes de Socio)
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        // Número de file (agrupa inscriptos que viajan juntos)
        public string NumeroFile { get; set; } = string.Empty;

        // Relación con variante
        public int VarianteViajeId { get; set; }
        public VarianteViaje Variante { get; set; } = null!;

        // Metadata financiera
        public decimal MontoAbonado { get; set; }
        public decimal MontoPendiente { get; set; }
        public bool Cancelado { get; set; } = false;

        // Historial de pagos
        public List<PagoInscriptoViaje> HistorialPagos { get; set; } = new List<PagoInscriptoViaje>();
    }
}
```

> **IMPORTANTE**: Eliminar el `using APIClub.Domain.GestionSocios.Models;` del archivo, ya que la relación con `Socio` se elimina por completo. También notar que se estandarizaron las propiedades a PascalCase (`montoAbonado` → `MontoAbonado`, `cancelado` → `Cancelado`). Esto implica buscar TODOS los usos de esos nombres con minúsculas en el codebase y renombrarlos.

### Cambios en `AppDbcontext.cs`:

**Eliminar** la configuración actual de la relación InscriptoViaje → Socio:
```csharp
// ELIMINAR este bloque:
entity.HasOne(iv => iv.Socio)
      .WithMany()
      .HasForeignKey(iv => iv.SocioId)
      .OnDelete(DeleteBehavior.Restrict);
```

**Agregar** la configuración de las nuevas propiedades y la relación con PagoInscriptoViaje:
```csharp
modelBuilder.Entity<InscriptoViaje>(entity =>
{
    entity.Property(iv => iv.Nombre).IsRequired().HasMaxLength(100);
    entity.Property(iv => iv.Apellido).IsRequired().HasMaxLength(100);
    entity.Property(iv => iv.Telefono).HasMaxLength(50);
    entity.Property(iv => iv.NumeroFile).IsRequired().HasMaxLength(50);
    entity.Property(iv => iv.MontoAbonado).HasColumnType("decimal(18,2)");
    entity.Property(iv => iv.MontoPendiente).HasColumnType("decimal(18,2)");

    entity.HasOne(iv => iv.Variante)
          .WithMany(vv => vv.Inscriptos)
          .HasForeignKey(iv => iv.VarianteViajeId)
          .OnDelete(DeleteBehavior.Restrict);

    entity.HasMany(iv => iv.HistorialPagos)
          .WithOne()
          .HasForeignKey(p => p.InscriptoViajeId)
          .OnDelete(DeleteBehavior.Cascade);
});
```

### Cambios en `IViajeReadRepository.cs`:

**Eliminar** el método `EstaInscripto(int socioId, int varianteViajeId)` ya que no existe más la relación con Socio.

### Cambios en `ViajeReadRepository.cs`:

1. **Eliminar** el método `EstaInscripto` completo.
2. **Modificar** `GetViajeCompleto` eliminando el `.ThenInclude(i => i.Socio)`:

```csharp
public async Task<Viaje?> GetViajeCompleto(int id)
{
    return await _dbContext.Viajes
        .Include(v => v.Variantes)
            .ThenInclude(vv => vv.Inscriptos)
                .ThenInclude(i => i.HistorialPagos)
        .AsNoTracking()
        .FirstOrDefaultAsync(v => v.Id == id);
}
```

> **Nota**: se reemplaza `.ThenInclude(i => i.Socio)` por `.ThenInclude(i => i.HistorialPagos)` para cargar el historial de pagos en la consulta.

3. **Agregar** un nuevo método para obtener un inscripto con su historial de pagos:

```csharp
// en la interfaz IViajeReadRepository:
Task<InscriptoViaje?> GetInscriptoWithPagos(int id);

// en la implementación ViajeReadRepository:
public async Task<InscriptoViaje?> GetInscriptoWithPagos(int id)
{
    return await _dbContext.Inscriptos
        .Include(i => i.HistorialPagos)
        .Include(i => i.Variante)
        .FirstOrDefaultAsync(i => i.Id == id);
}
```

---

## Cambio 2: Concepto de "File" (número de file)

### Contexto
El "file" es un string aleatorio que identifica a inscriptos que viajan juntos (familia, grupo de amigos). Un file puede pertenecer a una sola persona o a varias. **No** es un ID autoincremental, es ingresado manualmente. No hace falta crear una tabla separada para grupos; simplemente se agrega el campo `NumeroFile` a la tabla `InscriptoViaje` y la agrupación se hace en el frontend al mostrar las tablas.

### Implementación
- Ya incluido en el modelo `InscriptoViaje.cs` del Cambio 1 como `public string NumeroFile { get; set; }`.
- En la configuración de `AppDbcontext.cs` se marca como `.IsRequired().HasMaxLength(50)`.
- **No se crea una tabla ni entidad adicional para el grupo/file.** La agrupación es visual en el frontend.

### Impacto en DTOs

**`InsertInscriptoViajeDto.cs`** — se detalla completo en el Cambio 3.

**`InscriptosDto.cs`** — Estado deseado:
```csharp
namespace APIClub.Application.Dtos.Viajes.Views
{
    public class InscriptosDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string NumeroFile { get; set; } = string.Empty;
        public decimal MontoAbonado { get; set; }
        public decimal MontoPendiente { get; set; }
        public bool Cancelado { get; set; }
        public List<PagoInscriptoDto> HistorialPagos { get; set; } = new();
    }
}
```

> **Notar**: se eliminan `NombreSocio`, `DniSocio`, `TelefonoSocio` y se reemplazan por `Nombre`, `Apellido`, `Telefono`. Se agrega `NumeroFile` y `HistorialPagos`.

### Impacto en `ViajesService.cs` — método `VerViajeCompleto`:

Dentro del mapeo de inscriptos, cambiar:
```csharp
// ANTES:
Inscriptos = vv.Inscriptos
    .OrderBy(i => i.Socio.Apellido)
    .ThenBy(i => i.Socio.Nombre)
    .Select(i => new InscriptosDto
    {
        Id = i.Id,
        NombreSocio = $"{i.Socio.Apellido} {i.Socio.Nombre}",
        DniSocio = i.Socio.Dni,
        TelefonoSocio = i.Socio.Telefono ?? string.Empty,
        MontoAbonado = i.montoAbonado,
        MontoPendiente = i.MontoPendiente,
        Cancelado = i.cancelado
    }).ToList()

// DESPUÉS:
Inscriptos = vv.Inscriptos
    .OrderBy(i => i.NumeroFile)
    .ThenBy(i => i.Apellido)
    .ThenBy(i => i.Nombre)
    .Select(i => new InscriptosDto
    {
        Id = i.Id,
        Nombre = i.Nombre,
        Apellido = i.Apellido,
        Telefono = i.Telefono,
        NumeroFile = i.NumeroFile,
        MontoAbonado = i.MontoAbonado,
        MontoPendiente = i.MontoPendiente,
        Cancelado = i.Cancelado,
        HistorialPagos = i.HistorialPagos
            .OrderByDescending(p => p.FechaPago)
            .Select(p => new PagoInscriptoDto
            {
                Id = p.Id,
                Monto = p.Monto,
                FechaPago = p.FechaPago,
                NumeroRecibo = p.NumeroRecibo
            }).ToList()
    }).ToList()
```

> **Nota**: los inscriptos ahora se ordenan primero por `NumeroFile` para que queden agrupados visualmente los que comparten un file.

---

## Cambio 3: Nuevo formulario de inscripción (carga por file + múltiples personas)

### Contexto
El formulario anterior buscaba un socio por DNI. El nuevo flujo es:
1. **Paso 1**: Ingresar el número de file + cargar uno o varios inscriptos (nombre, apellido, teléfono).
2. **Paso 2**: Seleccionar la variante del viaje para cada inscripto. Si todos eligen la misma, hay un botón para aplicarla a todos.
3. **Paso 3**: Ingresar el monto de entrega inicial para cada inscripto individualmente (con la validación de que no sea menor a la seña de su variante elegida).

### Nuevo DTO — `InsertInscriptoViajeDto.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Create
{
    public class InsertInscriptoViajeDto
    {
        [Required(ErrorMessage = "El número de file es obligatorio")]
        public string NumeroFile { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar al menos un inscripto")]
        [MinLength(1, ErrorMessage = "Debe ingresar al menos un inscripto")]
        public List<InscriptoItemDto> Inscriptos { get; set; } = new();
    }

    public class InscriptoItemDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar una variante")]
        public int VarianteViajeId { get; set; }

        [Required(ErrorMessage = "El monto abonado es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal MontoAbonado { get; set; }
    }
}
```

> **IMPORTANTE**: el DTO ahora es un wrapper con `NumeroFile` y una lista de `InscriptoItemDto`. Esto permite inscribir a una o varias personas bajo el mismo file en una sola petición.

### Cambios en `IViajesServices.cs`:

Cambiar la firma del método:
```csharp
// ANTES:
Task<Result<object?>> InscriptSocioToViaje(InsertInscriptoViajeDto dto);

// DESPUÉS:
Task<Result<object?>> InscribirPersonasAlViaje(InsertInscriptoViajeDto dto);
```

### Cambios en `ViajesService.cs` — método de inscripción:

Reemplazar el método `InscriptSocioToViaje` completo por:
```csharp
public async Task<Result<object?>> InscribirPersonasAlViaje(InsertInscriptoViajeDto dto)
{
    try
    {
        // Validar cada inscripto
        foreach (var item in dto.Inscriptos)
        {
            var variante = await _viajeReadRepository.GetVarianteById(item.VarianteViajeId);
            if (variante == null)
                return Result<object?>.Error(
                    $"La variante seleccionada para {item.Nombre} {item.Apellido} no existe.", 404);

            if (item.MontoAbonado < variante.ValorSeña)
                return Result<object?>.Error(
                    $"El monto para {item.Nombre} {item.Apellido} debe ser al menos igual a la seña ({variante.ValorSeña}).", 400);

            if (item.MontoAbonado > variante.ValorViaje)
                return Result<object?>.Error(
                    $"El monto para {item.Nombre} {item.Apellido} no puede superar el valor del viaje ({variante.ValorViaje}).", 400);
        }

        // Crear inscriptos
        foreach (var item in dto.Inscriptos)
        {
            var variante = await _viajeReadRepository.GetVarianteById(item.VarianteViajeId);
            var montoPendiente = variante!.ValorViaje - item.MontoAbonado;

            var inscripto = new InscriptoViaje
            {
                Nombre = item.Nombre,
                Apellido = item.Apellido,
                Telefono = item.Telefono,
                NumeroFile = dto.NumeroFile,
                VarianteViajeId = item.VarianteViajeId,
                MontoAbonado = item.MontoAbonado,
                MontoPendiente = montoPendiente,
            };

            // La entrega inicial también cuenta como pago registrado
            inscripto.HistorialPagos.Add(new PagoInscriptoViaje
            {
                Monto = item.MontoAbonado,
                FechaPago = DateOnly.FromDateTime(DateTime.Now),
                NumeroRecibo = "ENTREGA-INICIAL"
            });

            variante.Inscriptos.Add(inscripto);
        }

        bool success = await _unitOfWork.SaveChangesAsync();

        if (!success)
            return Result<object?>.Error("Hubo un error al inscribir a las personas.", 500);

        return Result<object?>.Exito(null);
    }
    catch (Exception)
    {
        return Result<object?>.Error("Lo sentimos, ocurrió un error inesperado al realizar la inscripción.", 500);
    }
}
```

### Cambios en `ViajesController.cs`:

```csharp
// ANTES:
[HttpPost("inscribir")]
public async Task<IActionResult> InscribirSocioToViaje([FromBody] InsertInscriptoViajeDto dto)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);
    var result = await _viajesService.InscriptSocioToViaje(dto);
    ...
}

// DESPUÉS:
[HttpPost("inscribir")]
public async Task<IActionResult> InscribirPersonasAlViaje([FromBody] InsertInscriptoViajeDto dto)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);
    var result = await _viajesService.InscribirPersonasAlViaje(dto);
    if (!result.Exit)
    {
        return StatusCode(result.Errorcode, result.Errormessage);
    }
    return Ok(result.Data);
}
```

### Cambios en el Frontend — `viajesService.js`:

Renombrar método (la URL no cambia, solo el nombre del método JS y la estructura del body):
```javascript
// ANTES:
async inscribirSocio(dto) { ... }

// DESPUÉS:
async inscribirPersonas(dto) {
    // dto tiene forma: { numeroFile: "ABC123", inscriptos: [{ nombre, apellido, telefono, varianteViajeId, montoAbonado }, ...] }
    const response = await fetch(`${API_URL}/inscribir`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(dto),
    });
    if (!response.ok) {
        const msg = await handleError(response, "Error al inscribir personas al viaje");
        throw new Error(msg);
    }
},
```

### Cambios en Frontend — `InscripcionConfirmModal.vue`:

**REESCRIBIR COMPLETO**. El nuevo componente debe implementar un wizard de 3 pasos:

**Paso 1 — File + Personas:**
- Input para el número de file (string libre).
- Formulario para agregar inscriptos (nombre, apellido, teléfono) con un botón "Agregar otra persona". Se renderizan como cards/lista con botón de eliminar.
- Botón "Siguiente" (habilitado solo si hay al menos 1 persona cargada y se ingresó un file).

**Paso 2 — Variantes:**
- Mostrar las variantes disponibles del viaje (ya se cargan con `ViajesService.listarVariantesDeViaje`).
- Para cada inscripto cargado en el Paso 1, mostrar un selector de variante.
- Incluir un botón "Aplicar a todos" que pone la misma variante para todos los inscriptos.
- Botón "Anterior" y "Siguiente".

**Paso 3 — Montos:**
- Para cada inscripto, mostrar:
  - Nombre completo de la persona
  - Nombre de la variante elegida
  - Valor de la seña (mínimo a pagar)
  - Valor total del viaje
  - Input para ingresar el monto de entrega inicial
- **Validación**: el monto no puede ser menor a la seña de la variante elegida ni mayor al valor del viaje.
- Botón "Anterior" y "Confirmar Inscripción".

**Al confirmar**, llamar a `ViajesService.inscribirPersonas(dto)` con el formato:
```javascript
{
  numeroFile: "ABC123",
  inscriptos: [
    {
      nombre: "Juan",
      apellido: "Pérez",
      telefono: "3415551234",
      varianteViajeId: 5,
      montoAbonado: 50000
    },
    // ... más inscriptos
  ]
}
```

> **NOTA sobre estilo**: Mantener la misma estética del wizard actual (bordes redondeados, badges de paso, sombras suaves, tipografía limpia). El formulario por pasos actual ya se consideraba estético — conservar esa línea.

---

## Cambio 4: Historial de pagos del inscripto

### Contexto
Antes solo se registraba un `montoAbonado` y `montoPendiente` acumulado. Ahora se necesita **un registro detallado de cada pago** individual: fecha, monto, número de recibo. La entrega inicial también es un pago y debe registrarse.

### Nuevo modelo — `PagoInscriptoViaje.cs` (ARCHIVO NUEVO):

Crear en `APIClub/Domain/ModuloGestionViajes/Models/PagoInscriptoViaje.cs`:
```csharp
namespace APIClub.Domain.ModuloGestionViajes.Models
{
    public class PagoInscriptoViaje
    {
        public int Id { get; set; }
        public int InscriptoViajeId { get; set; }
        public decimal Monto { get; set; }
        public DateOnly FechaPago { get; set; }
        public string NumeroRecibo { get; set; } = string.Empty;
    }
}
```

### Nuevo DTO — `PagoInscriptoDto.cs` (ARCHIVO NUEVO):

Crear en `APIClub/Application/Dtos/Viajes/Views/PagoInscriptoDto.cs`:
```csharp
namespace APIClub.Application.Dtos.Viajes.Views
{
    public class PagoInscriptoDto
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateOnly FechaPago { get; set; }
        public string NumeroRecibo { get; set; } = string.Empty;
    }
}
```

### Configuración en `AppDbcontext.cs`:

Agregar la configuración de la nueva entidad:
```csharp
modelBuilder.Entity<PagoInscriptoViaje>(entity =>
{
    entity.Property(p => p.Monto).HasColumnType("decimal(18,2)");
    entity.Property(p => p.NumeroRecibo).IsRequired().HasMaxLength(100);
    entity.Property(p => p.FechaPago)
          .HasConversion(
              v => v.ToDateTime(new TimeOnly(0, 0)),
              v => DateOnly.FromDateTime(v));
});
```

Y agregar el `DbSet`:
```csharp
public DbSet<PagoInscriptoViaje> PagosInscriptosViaje { get; set; }
```

> La relación `InscriptoViaje → PagoInscriptoViaje` ya se define en la configuración de `InscriptoViaje` del Cambio 1.

### Modificar `UpdatePagoViajeDto.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Update
{
    public class UpdatePagoViajeDto
    {
        public int IdInscripto { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el monto abonado")]
        public decimal MontoAbonado { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el número de recibo")]
        public string NumeroRecibo { get; set; } = string.Empty;
    }
}
```

### Modificar `ViajesService.cs` — método `ActualizarPagoDeViaje`:

Actualizar la firma y la lógica:
```csharp
// Cambiar firma en la interfaz IViajesServices:
Task<Result<object?>> ActualizarPagoDeViaje(int IdInscripto, decimal montoAbonado, string numeroRecibo);

// Implementación:
public async Task<Result<object?>> ActualizarPagoDeViaje(int IdInscripto, decimal montoAbonado, string numeroRecibo)
{
    try
    {
        var inscripto = await _viajeReadRepository.GetInscriptoWithPagos(IdInscripto);

        if (inscripto == null)
            return Result<object?>.NotFound("Lo sentimos, no se encontró el inscripto.");

        if (montoAbonado > inscripto.MontoPendiente)
            return Result<object?>.Error(
                $"El monto a abonar (${montoAbonado}) no puede ser mayor al saldo pendiente (${inscripto.MontoPendiente}).", 400);

        // Actualizar totales
        inscripto.MontoAbonado += montoAbonado;
        inscripto.MontoPendiente -= montoAbonado;

        // Registrar el pago en el historial
        inscripto.HistorialPagos.Add(new PagoInscriptoViaje
        {
            Monto = montoAbonado,
            FechaPago = DateOnly.FromDateTime(DateTime.Now),
            NumeroRecibo = numeroRecibo
        });

        bool success = await _unitOfWork.SaveChangesAsync();

        if (!success)
            return Result<object?>.Error("Hubo un error al procesar el pago.", 500);

        return Result<object?>.Exito(null);
    }
    catch (Exception)
    {
        return Result<object?>.Error("Lo sentimos, hubo un error inesperado al actualizar el pago.", 500);
    }
}
```

### Modificar `ViajesController.cs` — endpoint de pago:

```csharp
[HttpPost("pago")]
public async Task<IActionResult> ActualizarPagoDeViaje([FromBody] UpdatePagoViajeDto dto)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var result = await _viajesService.ActualizarPagoDeViaje(dto.IdInscripto, dto.MontoAbonado, dto.NumeroRecibo);
    if (!result.Exit)
    {
        return StatusCode(result.Errorcode, result.Errormessage);
    }
    return Ok();
}
```

### Cambios en el Frontend — `PagoViajeModal.vue`:

Agregar un input para el **número de recibo** (campo obligatorio) al formulario existente. El payload enviado al backend ahora incluye `numeroRecibo`:
```javascript
await ViajesService.actualizarPago({
    idInscripto: props.inscripto.id,
    montoAbonado: montoAbonar.value,
    numeroRecibo: numeroRecibo.value,
});
```

### Cambios en el Frontend — `ViajeDetailView.vue`:

En la tabla de inscriptos, agregar la posibilidad de expandir una fila para ver el historial de pagos del inscripto. Cada fila del historial muestra: **Fecha**, **Monto**, **Nro. Recibo**. Implementar como un sub-acordeón o fila expandible debajo de cada inscripto.

La columna `DNI` que actualmente existe en la tabla de inscriptos se elimina. Las columnas quedarán:
- File
- Apellido y Nombre (separar del anterior "Socio")
- Teléfono
- Abonado
- Pendiente
- Estado
- Acciones

Los inscriptos deben mostrarse **agrupados por file** cuando un file tiene más de un inscripto. Esto es puramente visual, usando un separador o un encabezado de grupo. La agrupación ya viene hecha desde el backend porque los inscriptos se ordenan por `NumeroFile`.

---

## Cambio 5: Cancelación de viaje con pago completo automático

### Contexto
Cuando un inscripto cancela su viaje, **debe pagar el total del viaje**. No se hace un registro manual del pago; al cancelar, el sistema debe:
1. Marcar `Cancelado = true`.
2. Poner `MontoAbonado` en el valor total del viaje de su variante.
3. Poner `MontoPendiente` en 0.
4. Registrar en el historial de pagos un pago automático por el saldo pendiente con una nota indicativa.

Esto asegura que en el resumen del viaje, el dinero de esa cancelación se sume al total recaudado y se tome en cuenta para el cálculo de comisión y pago a agencia.

### Comportamiento actual (verificado en el código):
Actualmente `CancelarViajeDeSocio` hace un **soft delete**: pone `cancelado = true` pero **no modifica los montos**. Luego en `VerViajeCompleto`, los inscriptos cancelados se excluyen del cálculo:
```csharp
var inscriptosActivos = todosLosInscriptos.Where(i => !i.cancelado).ToList();
var totalRecaudado = inscriptosActivos.Sum(i => i.montoAbonado);
```
Esto significa que actualmente el dinero de un inscripto cancelado **NO** se cuenta como recaudado.

### Modificar `ViajesService.cs` — método `CancelarViajeDeSocio`:

Renombrar a `CancelarInscripcionDeViaje` y cambiar la lógica:

```csharp
// Cambiar firma en la interfaz:
Task<Result<object?>> CancelarInscripcionDeViaje(int idInscripto);

// Implementación:
public async Task<Result<object?>> CancelarInscripcionDeViaje(int idInscripto)
{
    try
    {
        var inscripto = await _viajeReadRepository.GetInscriptoWithPagos(idInscripto);

        if (inscripto == null)
            return Result<object?>.NotFound("Lo sentimos no se pudo encontrar el inscripto");

        if (inscripto.Cancelado)
            return Result<object?>.Error("Este inscripto ya fue cancelado.", 400);

        // Si tiene saldo pendiente, registrar un pago automático por el total pendiente
        if (inscripto.MontoPendiente > 0)
        {
            var saldoPendiente = inscripto.MontoPendiente;

            inscripto.HistorialPagos.Add(new PagoInscriptoViaje
            {
                Monto = saldoPendiente,
                FechaPago = DateOnly.FromDateTime(DateTime.Now),
                NumeroRecibo = "CANCELACION-PAGO-COMPLETO"
            });

            inscripto.MontoAbonado += saldoPendiente;
            inscripto.MontoPendiente = 0;
        }

        inscripto.Cancelado = true;

        bool success = await _unitOfWork.SaveChangesAsync();

        if (!success)
            return Result<object?>.Error("Lo sentimos hubo un error al cancelar la inscripción", 500);

        return Result<object?>.Exito(null);
    }
    catch (Exception)
    {
        return Result<object?>.Error("Lo sentimos hubo un error inesperado al cancelar la inscripción", 500);
    }
}
```

### Modificar cálculos de resumen en `ViajesService.cs` — `VerViajeCompleto`:

Los cálculos de totales deben **incluir a los inscriptos cancelados** en el recaudado:

```csharp
// ANTES:
var inscriptosActivos = todosLosInscriptos.Where(i => !i.cancelado).ToList();
var totalInscriptos = inscriptosActivos.Count;
var totalCancelados = todosLosInscriptos.Count(i => i.cancelado);
var totalRecaudado = inscriptosActivos.Sum(i => i.montoAbonado);
var totalPendiente = inscriptosActivos.Sum(i => i.MontoPendiente);

// DESPUÉS:
var totalInscriptos = todosLosInscriptos.Count(i => !i.Cancelado);
var totalCancelados = todosLosInscriptos.Count(i => i.Cancelado);

// El recaudado incluye TODOS los inscriptos (activos + cancelados con pago completo)
var totalRecaudado = todosLosInscriptos.Sum(i => i.MontoAbonado);
// El pendiente solo considera los activos (los cancelados ya tienen MontoPendiente = 0)
var totalPendiente = todosLosInscriptos.Where(i => !i.Cancelado).Sum(i => i.MontoPendiente);
```

Ajustar el cálculo de comisión y agencia (la fórmula no cambia, solo se recalcula sobre el totalRecaudado actualizado que ahora incluye los cancelados):
```csharp
var montoComision = totalRecaudado * (viaje.PorcentajeComision / 100);
var montoParaAgencia = totalRecaudado - montoComision;
```

### Cambios en `ViajesController.cs`:

Actualizar la referencia:
```csharp
[HttpDelete("inscripcion/{idInscripto}")]
public async Task<IActionResult> CancelarInscripcionDeViaje(int idInscripto)
{
    var result = await _viajesService.CancelarInscripcionDeViaje(idInscripto);
    ...
}
```

### Cambios en Frontend — `ViajeDetailView.vue`:

Actualizar el texto del modal de confirmación de cancelación para informar que la cancelación implica el pago completo del viaje:
```
"¿Está seguro que desea cancelar esta inscripción? 
Al cancelar, se registrará automáticamente el pago completo del viaje. 
El monto total quedará como recaudado."
```

---

## Migración de Base de Datos

Una vez que todos los cambios de modelo y DbContext estén aplicados, ejecutar:

```bash
cd APIClub
dotnet ef migrations add RefactorizarModuloViajes
dotnet ef database update
```

### Consideraciones de migración:
- La columna `SocioId` se elimina de la tabla `Inscriptos`.
- Se agregan columnas `Nombre`, `Apellido`, `Telefono`, `NumeroFile` a la tabla `Inscriptos`.
- Se renombran `montoAbonado` → `MontoAbonado` y `cancelado` → `Cancelado` (EF puede manejar esto como rename o drop+add, verificar la migración generada).
- Se crea la nueva tabla `PagosInscriptosViaje`.
- **CUIDADO**: Si hay datos existentes en la tabla `Inscriptos`, la migración fallará porque las nuevas columnas `Nombre`, `Apellido` y `NumeroFile` son `NOT NULL`. Se debe decidir si limpiar los datos antes de migrar o agregar valores default en la migración.

### Estrategia recomendada para datos existentes:
Antes de aplicar la migración, editar el archivo de migración generado para agregar valores por defecto para los datos existentes:
```csharp
migrationBuilder.Sql("UPDATE Inscriptos SET Nombre = 'Migrado', Apellido = 'Migrado', Telefono = '', NumeroFile = 'LEGACY-' + CAST(Id AS NVARCHAR(10))");
```

---

## Orden de Ejecución Recomendado

1. **Backend - Modelos**: Crear `PagoInscriptoViaje.cs` y modificar `InscriptoViaje.cs`
2. **Backend - DbContext**: Actualizar `AppDbcontext.cs` (nueva entidad, nueva config, eliminar relación Socio)
3. **Backend - DTOs**: Crear `PagoInscriptoDto.cs`, modificar `InsertInscriptoViajeDto.cs`, `InscriptosDto.cs`, `UpdatePagoViajeDto.cs`, `FullViewViajeDto.cs`
4. **Backend - Repositories**: Modificar interfaces y sus implementaciones
5. **Backend - Service**: Modificar `ViajesService.cs` (todos los métodos afectados)
6. **Backend - Controller**: Modificar `ViajesController.cs`
7. **Migración**: Generar y aplicar migración EF Core
8. **Frontend - Service**: Modificar `viajesService.js`
9. **Frontend - Components**: Reescribir `InscripcionConfirmModal.vue`, modificar `PagoViajeModal.vue`
10. **Frontend - Views**: Modificar `ViajeDetailView.vue`
11. **Testing**: Verificar el flujo completo (inscripción, pagos, cancelación, resumen)

---

## Checklist de Verificación Final

- [ ] Un inscripto NO tiene relación con la tabla Socios
- [ ] Al inscribir, se ingresan nombre, apellido y teléfono manualmente
- [ ] El campo DNI no aparece en ningún formulario ni tabla del módulo de viajes
- [ ] El número de file se carga al inscribir y se muestra en las tablas
- [ ] Los inscriptos con el mismo file aparecen agrupados visualmente
- [ ] Se pueden inscribir múltiples personas bajo el mismo file en una sola operación
- [ ] Al inscribir, se puede asignar una variante por persona o la misma para todos
- [ ] El monto de entrega de cada inscripto se valida contra la seña de su variante
- [ ] Cada pago queda registrado con fecha, monto y número de recibo
- [ ] La entrega inicial aparece en el historial de pagos
- [ ] Al cancelar un viaje, el saldo pendiente se marca como pagado automáticamente
- [ ] Los inscriptos cancelados se suman al total recaudado en el resumen del viaje
- [ ] La comisión y el monto para agencia se calculan sobre el recaudado total (incluyendo cancelados)
- [ ] El formulario de inscripción mantiene la estética de wizard por pasos

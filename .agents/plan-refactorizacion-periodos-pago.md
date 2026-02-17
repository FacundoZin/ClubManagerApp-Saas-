# 🔧 Plan de Refactorización: Períodos de Pago Configurables (SaaS-Ready)

## 📋 Resumen Ejecutivo

El sistema actual tiene **lógica de períodos semestrales hardcodeada** en al menos **15+ ubicaciones** a lo largo de las 3 capas (Domain, Application, Infrastructure). La refactorización busca extraer toda esta lógica a un **modelo de configuración dinámico** que cada cliente SaaS pueda personalizar (mensual, bimestral, trimestral, cuatrimestral, semestral, anual).

---

## 🔍 Diagnóstico: Puntos de Hardcodeo Encontrados

### 🔴 Severidad ALTA (lógica de negocio directamente afectada)

| #   | Archivo                                               | Línea(s) | Código Hardcodeado                        | Impacto                                                                  |
| --- | ----------------------------------------------------- | -------- | ----------------------------------------- | ------------------------------------------------------------------------ |
| 1   | `Domain/ModuloGestionCuotas/Models/Cuota.cs`          | 13       | `public int Semestre { get; set; }`       | El modelo de dominio **solo soporta semestres** como concepto de período |
| 2   | `Domain/PaymentsOnline/Modelos/PaymentToken.cs`       | 8-9      | `public int anio` + `public int semestre` | Token de pago **atado a semestre**                                       |
| 3   | `Application/Dtos/Socios/PeriodoAdeudadoDto.cs`       | 5-6      | `Anio` + `Semestre`                       | DTO referenciado en toda la app                                          |
| 4   | `Application/Dtos/Socios/PeriodoCuotasDto.cs`         | 10-11    | `anio` + `semestre`                       | DTO de historial de cuotas                                               |
| 5   | `Application/Dtos/Cuota/CuotaConSocioDto.cs`          | 11-12    | `Anio` + `Semestre`                       | DTO de consulta de cuotas                                                |
| 6   | `Application/Dtos/Cuota/HistorialCuotasRequestDto.cs` | 12       | `public int? Semestre`                    | Filtro de búsqueda atado a semestre                                      |
| 7   | `Application/Dtos/Payment/PortalPaymentViewDto.cs`    | 7        | `public string semestrePago`              | Vista del portal de pago                                                 |
| 8   | `Application/Dtos/Payment/InfoComprobanteDto.cs`      | 10       | `public string semestrePagoText`          | Comprobante de pago                                                      |

### 🟠 Severidad MEDIA (lógica de cálculo de períodos)

| #   | Archivo                                           | Línea(s)               | Código Hardcodeado                                                               | Descripción                                                                                                                          |
| --- | ------------------------------------------------- | ---------------------- | -------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| 9   | `Application/Services/SociosManagmentService.cs`  | 95, 100, 243, 250, 338 | `Month <= 6 ? 1 : 2` (×5 veces)                                                  | Cálculo de semestre actual en múltiples métodos: `GetSocioByDni()`, `GetFullSocioById()`, `GetPadronSocios()`, `GetSociosDeudores()` |
| 10  | `Application/Services/SociosManagmentService.cs`  | 102-118, 252-283       | Loops `for (int anio = anioInicio...)` con `semestreDesde/semestreHasta` (1 a 2) | Generación de períodos adeudados iterando solo por semestres (máximo 2 por año)                                                      |
| 11  | `Application/Services/PaymentTokenService.cs`     | 21                     | `now.Month <= 6 ? 1 : 2`                                                         | Determinación del semestre actual para tokens                                                                                        |
| 12  | `Application/Services/PaymentTokenService.cs`     | 98, 101                | `FechaAsociacion.Month <= 6 ? 1 : 2` + `((token.anio - AnioAsociacion) * 2)`     | Cálculo de cuotas que deberían haberse pagado, multiplicando por **2** (semestres por año)                                           |
| 13  | `Application/Services/NotificacionsService.cs`    | 28                     | `fechaActual.Month <= 6 ? 1 : 2`                                                 | Notificaciones por WhatsApp del período actual                                                                                       |
| 14  | `Application/Services/CobranzasService.cs`        | 51                     | `hoy.Month <= 6 ? 1 : 2`                                                         | Listar deudores por lote                                                                                                             |
| 15  | `Application/Helpers/PaymentDescriptionHelper.cs` | 5-18                   | `GetSemestreText()`, `BuildCuotaDescription()`                                   | Textos descriptivos hardcodeados a "primer/segundo semestre"                                                                         |

### 🟡 Severidad MEDIA-BAJA (repositorios y queries)

| #   | Archivo                                                          | Línea(s)      | Código Hardcodeado                                                                       | Descripción                                           |
| --- | ---------------------------------------------------------------- | ------------- | ---------------------------------------------------------------------------------------- | ----------------------------------------------------- |
| 16  | `Infrastructure/Persistence/Repositorio/SociosRepository.cs`     | 82            | `FechaAsociacion.Month <= 6 ? 1 : 2`                                                     | Filtrado de cuotas al traer socio con cuotas          |
| 17  | `Infrastructure/Persistence/Repositorio/SociosRepository.cs`     | 85            | `.Where(c => c.Semestre >= semestreIngreso)`                                             | Filtrado de cuotas por semestre de ingreso            |
| 18  | `Infrastructure/Persistence/Repositorio/SociosRepository.cs`     | 89-97, 99-116 | `GetSociosDeudores()` y `GetSociosDeudoresPaginado()` con `c.Semestre == semestreActual` | Queries de deudores filtran por semestre              |
| 19  | `Infrastructure/Persistence/Repositorio/SociosRepository.cs`     | 153-220       | `GetSociosDeudoresByLote()` — itera por semestres (1 a 2)                                | Generación de períodos adeudados con loop hardcodeado |
| 20  | `Infrastructure/Persistence/Repositorio/SociosRepository.cs`     | 222-235       | `GetSociosDeudoresWithPreferenceLinkDePagoPaginado()` con `c.Semestre == semestreActual` | Búsqueda de deudores para links de pago               |
| 21  | `Infrastructure/Persistence/Repositorio/CuotaRepository.cs`      | 73-100        | `ObtenerCuotasPorPeriodo(int anio, int semestre)`                                        | Consulta específica por semestre                      |
| 22  | `Application/Validators/PagoCuotaValidator.cs`                   | 38, 55        | `c.Anio == periodo.Anio && c.Semestre == periodo.Semestre`                               | Validación de pago duplicado compara por semestre     |
| 23  | `Infrastructure/Persistence/Repositorio/AnaliticasRepository.cs` | 174-189       | `GetTasaMorosidad(int anio, int semestre)` con `c.Semestre == semestre`                  | Analíticas filtran por semestre                       |

---

## 🏗️ Plan de Refactorización por Fases

---

### 📦 FASE 1: Crear el Modelo de Configuración de Períodos (Domain Layer)

**Objetivo**: Definir una entidad configurable que represente cualquier tipo de periodicidad.

#### 1.1 — Crear enum `TipoPeriodo`

**Archivo**: `APIClub/Domain/Enums/TipoPeriodo.cs`

```csharp
namespace APIClub.Domain.Enums
{
    public enum TipoPeriodo
    {
        Mensual = 1,       // 12 períodos por año
        Bimestral = 2,     // 6 períodos por año
        Trimestral = 3,    // 4 períodos por año
        Cuatrimestral = 4, // 3 períodos por año
        Semestral = 6,     // 2 períodos por año
        Anual = 12          // 1 período por año
    }
}
```

> **Nota**: Los valores numéricos representan la cantidad de meses que dura cada período. Esto permite hacer cálculos genéricos como `12 / (int)TipoPeriodo` para obtener la cantidad de períodos por año.

#### 1.2 — Crear modelo `ConfiguracionPeriodoPago`

**Archivo**: `APIClub/Domain/ModuloGestionCuotas/Models/ConfiguracionPeriodoPago.cs`

```csharp
namespace APIClub.Domain.ModuloGestionCuotas.Models
{
    public class ConfiguracionPeriodoPago
    {
        public int Id { get; set; }
        public TipoPeriodo TipoPeriodo { get; set; }
        public int DiaVencimiento { get; set; } // Día del mes en que vence la cuota
        public int DiasGracia { get; set; } // Días extra después de vencimiento
        public int DiasAnticipacionAviso { get; set; } // Días antes para enviar aviso
        public bool IsActive { get; set; } = true;
        public DateTime FechaCreacion { get; set; }

        // Propiedades calculadas
        public int MesesPorPeriodo => (int)TipoPeriodo;
        public int PeriodosPorAnio => 12 / MesesPorPeriodo;
    }
}
```

#### 1.3 — Crear servicio de dominio `PeriodoCalculator`

**Archivo**: `APIClub/Domain/ModuloGestionCuotas/Services/PeriodoCalculator.cs`

Este es el **corazón de la refactorización**. Toda la lógica de cálculo de períodos que estaba distribuida por toda la app se centraliza aquí.

```csharp
namespace APIClub.Domain.ModuloGestionCuotas.Services
{
    public class PeriodoCalculator
    {
        private readonly ConfiguracionPeriodoPago _config;

        public PeriodoCalculator(ConfiguracionPeriodoPago config)
        {
            _config = config;
        }

        /// <summary>
        /// Dado un mes del año (1-12), devuelve el número de período (1-based).
        /// Ejemplo: Config=Semestral → Enero=1, Julio=2
        ///          Config=Trimestral → Enero=1, Abril=2, Julio=3, Octubre=4
        /// </summary>
        public int ObtenerNumeroPeriodo(int mes)
        {
            return ((mes - 1) / _config.MesesPorPeriodo) + 1;
        }

        /// <summary>
        /// Devuelve el número de período actual basado en la fecha actual.
        /// Reemplaza: `now.Month <= 6 ? 1 : 2`
        /// </summary>
        public int ObtenerPeriodoActual()
        {
            return ObtenerNumeroPeriodo(DateTime.Now.Month);
        }

        /// <summary>
        /// Devuelve el número de período en el que un socio se asoció.
        /// Reemplaza: `FechaAsociacion.Month <= 6 ? 1 : 2`
        /// </summary>
        public int ObtenerPeriodoDeAsociacion(DateOnly fechaAsociacion)
        {
            return ObtenerNumeroPeriodo(fechaAsociacion.Month);
        }

        /// <summary>
        /// Genera todos los períodos que un socio debería haber pagado
        /// desde su fecha de asociación hasta el período actual.
        /// Reemplaza: los loops `for (int anio = anioInicio; anio <= anioActual; anio++) { for (int sem = semestreDesde; sem <= semestreHasta; sem++) {...} }`
        /// </summary>
        public List<(int Anio, int Periodo)> GenerarPeriodosDesdeAsociacion(
            DateOnly fechaAsociacion, int anioActual, int periodoActual)
        {
            var periodos = new List<(int Anio, int Periodo)>();
            int anioInicio = fechaAsociacion.Year;
            int periodoInicio = ObtenerPeriodoDeAsociacion(fechaAsociacion);
            int periodosPorAnio = _config.PeriodosPorAnio;

            for (int anio = anioInicio; anio <= anioActual; anio++)
            {
                int desde = (anio == anioInicio) ? periodoInicio : 1;
                int hasta = (anio == anioActual) ? periodoActual : periodosPorAnio;

                for (int p = desde; p <= hasta; p++)
                {
                    periodos.Add((anio, p));
                }
            }

            return periodos;
        }

        /// <summary>
        /// Calcula cuántos períodos deberían haberse pagado antes de un período dado.
        /// Reemplaza: `((token.anio - AnioAsociacion) * 2) + (token.semestre - SemestreAsociacion)`
        /// </summary>
        public int CalcularPeriodosAnteriores(
            DateOnly fechaAsociacion, int anioObjetivo, int periodoObjetivo)
        {
            int anioAsociacion = fechaAsociacion.Year;
            int periodoAsociacion = ObtenerPeriodoDeAsociacion(fechaAsociacion);
            int periodosPorAnio = _config.PeriodosPorAnio;

            return ((anioObjetivo - anioAsociacion) * periodosPorAnio)
                   + (periodoObjetivo - periodoAsociacion);
        }

        /// <summary>
        /// Genera un texto descriptivo del período.
        /// Reemplaza: `GetSemestreText()` → "primer semestre" / "segundo semestre"
        /// </summary>
        public string ObtenerTextoPeriodo(int numeroPeriodo)
        {
            int periodosPorAnio = _config.PeriodosPorAnio;

            return _config.TipoPeriodo switch
            {
                TipoPeriodo.Mensual => ObtenerNombreMes(numeroPeriodo),
                TipoPeriodo.Bimestral => $"bimestre {numeroPeriodo}",
                TipoPeriodo.Trimestral => $"trimestre {numeroPeriodo}",
                TipoPeriodo.Cuatrimestral => $"cuatrimestre {numeroPeriodo}",
                TipoPeriodo.Semestral => numeroPeriodo == 1 ? "primer semestre" : "segundo semestre",
                TipoPeriodo.Anual => "cuota anual",
                _ => $"período {numeroPeriodo}"
            };
        }

        /// <summary>
        /// Obtiene el mes de inicio de un período específico.
        /// Útil para filtros en queries.
        /// </summary>
        public int ObtenerMesInicioPeriodo(int numeroPeriodo)
        {
            return ((numeroPeriodo - 1) * _config.MesesPorPeriodo) + 1;
        }

        /// <summary>
        /// Obtiene el mes de fin de un período específico.
        /// </summary>
        public int ObtenerMesFinPeriodo(int numeroPeriodo)
        {
            return numeroPeriodo * _config.MesesPorPeriodo;
        }

        private string ObtenerNombreMes(int mes) => mes switch
        {
            1 => "enero", 2 => "febrero", 3 => "marzo",
            4 => "abril", 5 => "mayo", 6 => "junio",
            7 => "julio", 8 => "agosto", 9 => "septiembre",
            10 => "octubre", 11 => "noviembre", 12 => "diciembre",
            _ => $"mes {mes}"
        };
    }
}
```

#### 1.4 — Crear interfaz `IConfiguracionPeriodoRepository`

**Archivo**: `APIClub/Domain/ModuloGestionCuotas/Repositories/IConfiguracionPeriodoRepository.cs`

```csharp
namespace APIClub.Domain.ModuloGestionCuotas.Repositories
{
    public interface IConfiguracionPeriodoRepository
    {
        Task<ConfiguracionPeriodoPago> GetConfiguracionActiva();
        Task<ConfiguracionPeriodoPago> ActualizarConfiguracion(ConfiguracionPeriodoPago config);
    }
}
```

---

### 📦 FASE 2: Refactorizar el Modelo `Cuota` y DTOs (Domain + Application Layer)

**Objetivo**: Generalizar la propiedad `Semestre` a `NumeroPeriodo`.

#### 2.1 — Modificar modelo `Cuota`

**Archivo**: `APIClub/Domain/ModuloGestionCuotas/Models/Cuota.cs`

```csharp
public class Cuota
{
    public int Id { get; set; }
    public DateOnly FechaPago { get; set; }
    public decimal Monto { get; set; }
    public MetodosDePago FormaDePago { get; set; }
    public int Anio { get; set; }
    public int NumeroPeriodo { get; set; }  // ERA: Semestre

    // Relaciones
    public int SocioId { get; set; }
    public Socio? Socio { get; set; }
}
```

> ⚠️ **Migración de BD necesaria**: Renombrar columna `Semestre` → `NumeroPeriodo`. Se debe crear una migración de EF Core.

#### 2.2 — Modificar modelo `PaymentToken`

**Archivo**: `APIClub/Domain/PaymentsOnline/Modelos/PaymentToken.cs`

```csharp
public class PaymentToken
{
    public Guid Id { get; set; }
    public string nombreSocio { get; set; }
    public int IdSocio { get; set; }
    public int anio { get; set; }
    public int numeroPeriodo { get; set; }  // ERA: semestre
    public DateOnly FechaExpiracion { get; set; }
    public decimal monto { get; set; }
    public bool usado { get; set; } = false;
    public string? Preference_Id { get; set; }
    public string? PaymentStatus { get; set; }
    public string? StatusDetail { get; set; }
}
```

#### 2.3 — Refactorizar DTOs

Los siguientes DTOs deben cambiar `Semestre` → `NumeroPeriodo`:

| Archivo                        | Cambio                                             |
| ------------------------------ | -------------------------------------------------- |
| `PeriodoAdeudadoDto.cs`        | `Semestre` → `NumeroPeriodo`                       |
| `PeriodoCuotasDto.cs`          | `semestre` → `numeroPeriodo`                       |
| `CuotaConSocioDto.cs`          | `Semestre` → `NumeroPeriodo`                       |
| `HistorialCuotasRequestDto.cs` | `Semestre` → `NumeroPeriodo`                       |
| `PortalPaymentViewDto.cs`      | `semestrePago` → `periodoPago` (texto descriptivo) |
| `InfoComprobanteDto.cs`        | `semestrePagoText` → `periodoText`                 |

---

### 📦 FASE 3: Refactorizar Servicios de Aplicación

**Objetivo**: Inyectar `PeriodoCalculator` en cada servicio y reemplazar toda la lógica hardcodeada.

#### 3.1 — Crear un servicio de infraestructura que provea el `PeriodoCalculator`

**Archivo**: `APIClub/Application/Services/PeriodoProvider.cs`

```csharp
namespace APIClub.Application.Services
{
    public interface IPeriodoProvider
    {
        Task<PeriodoCalculator> GetCalculator();
        Task<int> GetPeriodoActual();
        Task<ConfiguracionPeriodoPago> GetConfiguracion();
    }

    public class PeriodoProvider : IPeriodoProvider
    {
        private readonly IConfiguracionPeriodoRepository _repo;
        private PeriodoCalculator? _cachedCalculator;

        public PeriodoProvider(IConfiguracionPeriodoRepository repo)
        {
            _repo = repo;
        }

        public async Task<PeriodoCalculator> GetCalculator()
        {
            if (_cachedCalculator == null)
            {
                var config = await _repo.GetConfiguracionActiva();
                _cachedCalculator = new PeriodoCalculator(config);
            }
            return _cachedCalculator;
        }

        public async Task<int> GetPeriodoActual()
        {
            var calc = await GetCalculator();
            return calc.ObtenerPeriodoActual();
        }

        public async Task<ConfiguracionPeriodoPago> GetConfiguracion()
        {
            return await _repo.GetConfiguracionActiva();
        }
    }
}
```

> **Registro de DI**: Registrar `IPeriodoProvider` como **Scoped** en `Program.cs`.

#### 3.2 — Refactorizar `SociosManagmentService`

**Cambios principales**:

1. Inyectar `IPeriodoProvider` en el constructor
2. Reemplazar **5 instancias** de `Month <= 6 ? 1 : 2` por `await _periodoProvider.GetPeriodoActual()`
3. Reemplazar los **2 loops de generación de períodos adeudados** por `calculator.GenerarPeriodosDesdeAsociacion()`

**Ejemplo de `GetSocioByDni()` refactorizado**:

```csharp
// ANTES:
int semestreActual = hoy.Month <= 6 ? 1 : 2;
int semestreInicio = socio.FechaAsociacion.Month <= 6 ? 1 : 2;
for (int anio = anioInicio; anio <= anioActual; anio++)
{
    int semestreDesde = (anio == anioInicio) ? semestreInicio : 1;
    int semestreHasta = (anio == anioActual) ? semestreActual : 2;
    for (int sem = semestreDesde; sem <= semestreHasta; sem++) { ... }
}

// DESPUÉS:
var calc = await _periodoProvider.GetCalculator();
int periodoActual = calc.ObtenerPeriodoActual();
var todosLosPeriodos = calc.GenerarPeriodosDesdeAsociacion(
    socio.FechaAsociacion, anioActual, periodoActual);

var periodosAdeudados = todosLosPeriodos
    .Where(p => !socio.HistorialCuotas.Any(c => c.Anio == p.Anio && c.NumeroPeriodo == p.Periodo))
    .Select(p => new PeriodoAdeudadoDto { Anio = p.Anio, NumeroPeriodo = p.Periodo })
    .ToList();
```

#### 3.3 — Refactorizar `PaymentTokenService`

**Cambios principales**:

1. Inyectar `IPeriodoProvider`
2. En `CreatePaymentTokens()`:
   - Reemplazar `now.Month <= 6 ? 1 : 2` → `calc.ObtenerPeriodoActual()`
   - Cambiar `semestre = semestreActual` → `numeroPeriodo = periodoActual`
3. En `ValidateToken()`:
   - Reemplazar `FechaAsociacion.Month <= 6 ? 1 : 2` → `calc.ObtenerPeriodoDeAsociacion()`
   - Reemplazar `((token.anio - AnioAsociacion) * 2) + ...` → `calc.CalcularPeriodosAnteriores()`

#### 3.4 — Refactorizar `CuotasService`

**Cambios**: En los 3 métodos de registro de pago, cambiar `Semestre = periodo.Semestre` → `NumeroPeriodo = periodo.NumeroPeriodo`

#### 3.5 — Refactorizar `PaymentService`

**Cambios**:

- Reemplazar todas las llamadas a `PaymentDescriptionHelper.GetSemestreText()` por `calc.ObtenerTextoPeriodo()`
- Reemplazar `BuildCuotaDescription()` y `BuildCuotaDescriptionShort()` por versiones que usen el `PeriodoCalculator`

#### 3.6 — Refactorizar `NotificacionsService`

**Cambios**:

- Reemplazar `fechaActual.Month <= 6 ? 1 : 2` → `calc.ObtenerPeriodoActual()`
- Reemplazar `semestre.GetSemestreText()` → `calc.ObtenerTextoPeriodo(periodo)`

#### 3.7 — Refactorizar `CobranzasService`

**Cambios**:

- Reemplazar `hoy.Month <= 6 ? 1 : 2` → `await _periodoProvider.GetPeriodoActual()`

#### 3.8 — Refactorizar `PaymentDescriptionHelper`

Hay dos opciones:

- **Opción A**: Eliminar la clase y usar `PeriodoCalculator.ObtenerTextoPeriodo()` en su lugar.
- **Opción B (recomendada)**: Refactorizar para que reciba el `PeriodoCalculator` como parámetro:

```csharp
public static class PaymentDescriptionHelper
{
    public static string BuildCuotaDescription(PeriodoCalculator calc, int periodo, int anio, string nombreOrganizacion)
    {
        string periodoText = calc.ObtenerTextoPeriodo(periodo);
        return $"cuota socio {nombreOrganizacion}, correspondiente al {periodoText} del año {anio}";
    }

    public static string BuildCuotaDescriptionShort(PeriodoCalculator calc, int periodo, int anio, string nombreOrganizacion)
    {
        string periodoText = calc.ObtenerTextoPeriodo(periodo);
        return $"cuota {nombreOrganizacion} correspondiente al {periodoText} del año {anio}";
    }
}
```

---

### 📦 FASE 4: Refactorizar Validadores

#### 4.1 — Refactorizar `PagoCuotaValidator`

**Cambios**:

- En `ValidarPagoEnEstablecimeinto()` y `ValidarPagoConCobrador()`:
  - Cambiar `c.Semestre == periodo.Semestre` → `c.NumeroPeriodo == periodo.NumeroPeriodo`

---

### 📦 FASE 5: Refactorizar Repositorios (Infrastructure Layer)

#### 5.1 — Refactorizar `SociosRepository`

**Cambios claves**:

1. **`GetSocioByIdWithCuotas()`** (línea 76-87):
   - Inyectar `IPeriodoProvider` en el repositorio o pasar el `PeriodoCalculator` como parámetro
   - Reemplazar `FechaAsociacion.Month <= 6 ? 1 : 2` → usar `PeriodoCalculator`
   - Cambiar filtro `.Where(c => c.Semestre >= semestreIngreso)` → `.Where(c => c.NumeroPeriodo >= periodoIngreso)`

2. **`GetSociosDeudores()`** y **`GetSociosDeudoresPaginado()`**:
   - Cambiar parámetros: `(int anioActual, int semestreActual)` → `(int anioActual, int periodoActual)`
   - Cambiar filtro: `c.Semestre == semestreActual` → `c.NumeroPeriodo == periodoActual`

3. **`GetSociosDeudoresByLote()`** (línea 153-220):
   - Cambiar parámetros: `semestreActual` → `periodoActual`
   - Reemplazar el loop de generación de períodos adeudados (líneas 186-204) por `PeriodoCalculator.GenerarPeriodosDesdeAsociacion()`
   - Cambiar `CuotasPagas.Any(c => c.Semestre == sem)` → `CuotasPagas.Any(c => c.NumeroPeriodo == p)`

4. **`GetSociosDeudoresWithPreferenceLinkDePagoPaginado()`**:
   - Cambiar parámetros y filtro de `Semestre` → `NumeroPeriodo`

5. **Cambios en `ISocioRepository` (interfaz)**: Los mismos cambios de firma aplican aquí.

#### 5.2 — Refactorizar `CuotaRepository`

**Cambios**:

- `ObtenerCuotasPorPeriodo(int anio, int semestre)` → `ObtenerCuotasPorPeriodo(int anio, int numeroPeriodo)`
- Cambiar filtro: `c.Semestre == semestre` → `c.NumeroPeriodo == numeroPeriodo`
- Actualizar `ICuotaRepository` con la misma firma.

#### 5.3 — Refactorizar `AnaliticasRepository`

**Cambios**:

- `GetTasaMorosidad(int anio, int semestre)` → `GetTasaMorosidad(int anio, int numeroPeriodo)`
- Cambiar filtro: `c.Semestre == semestre` → `c.NumeroPeriodo == numeroPeriodo`

#### 5.4 — Crear `ConfiguracionPeriodoRepository`

**Archivo**: `APIClub/Infrastructure/Persistence/Repositorio/ConfiguracionPeriodoRepository.cs`

```csharp
public class ConfiguracionPeriodoRepository : IConfiguracionPeriodoRepository
{
    private readonly AppDbcontext _context;

    public ConfiguracionPeriodoRepository(AppDbcontext context)
    {
        _context = context;
    }

    public async Task<ConfiguracionPeriodoPago> GetConfiguracionActiva()
    {
        return await _context.ConfiguracionPeriodoPago
            .FirstOrDefaultAsync(c => c.IsActive)
            ?? throw new InvalidOperationException("No hay configuración de período activa.");
    }

    public async Task<ConfiguracionPeriodoPago> ActualizarConfiguracion(ConfiguracionPeriodoPago config)
    {
        _context.ConfiguracionPeriodoPago.Update(config);
        await _context.SaveChangesAsync();
        return config;
    }
}
```

---

### 📦 FASE 6: Migración de Base de Datos

#### 6.1 — Agregar `ConfiguracionPeriodoPago` al `AppDbContext`

```csharp
public DbSet<ConfiguracionPeriodoPago> ConfiguracionPeriodoPago { get; set; }
```

#### 6.2 — Crear migración

```bash
dotnet ef migrations add RefactorizarPeriodosDePago
```

La migración debe:

1. Crear tabla `ConfiguracionPeriodoPago`
2. Renombrar columna `Semestre` → `NumeroPeriodo` en tabla `Cuotas`
3. Renombrar columna `semestre` → `numeroPeriodo` en tabla `PaymentTokens`
4. Insertar configuración por defecto (Semestral, para mantener compatibilidad):

```sql
INSERT INTO ConfiguracionPeriodoPago (TipoPeriodo, DiaVencimiento, DiasGracia, DiasAnticipacionAviso, IsActive, FechaCreacion)
VALUES (6, 1, 15, 10, 1, GETDATE());
```

---

### 📦 FASE 7: Registrar Servicios en DI

**Archivo**: `Program.cs` o donde se configure DI

```csharp
builder.Services.AddScoped<IConfiguracionPeriodoRepository, ConfiguracionPeriodoRepository>();
builder.Services.AddScoped<IPeriodoProvider, PeriodoProvider>();
```

---

### 📦 FASE 8: Endpoint de Configuración (opcional, para SaaS)

Crear un `ConfiguracionController` con endpoints:

- `GET /api/configuracion/periodos` — Obtener configuración actual
- `PUT /api/configuracion/periodos` — Actualizar configuración (solo admin)

---

## 📊 Tabla resumen de impacto por archivo

| Archivo                        | Cambios Estimados      | Prioridad |
| ------------------------------ | ---------------------- | --------- |
| `Cuota.cs`                     | Renombrar propiedad    | 🔴 Alta   |
| `PaymentToken.cs`              | Renombrar propiedad    | 🔴 Alta   |
| `PeriodoAdeudadoDto.cs`        | Renombrar propiedad    | 🔴 Alta   |
| `PeriodoCuotasDto.cs`          | Renombrar propiedad    | 🔴 Alta   |
| `CuotaConSocioDto.cs`          | Renombrar propiedad    | 🟠 Media  |
| `HistorialCuotasRequestDto.cs` | Renombrar propiedad    | 🟠 Media  |
| `PortalPaymentViewDto.cs`      | Renombrar propiedad    | 🟠 Media  |
| `InfoComprobanteDto.cs`        | Renombrar propiedad    | 🟠 Media  |
| `SociosManagmentService.cs`    | ~40 líneas de cambio   | 🔴 Alta   |
| `PaymentTokenService.cs`       | ~25 líneas de cambio   | 🔴 Alta   |
| `CuotasService.cs`             | ~15 líneas de cambio   | 🔴 Alta   |
| `PaymentService.cs`            | ~20 líneas de cambio   | 🔴 Alta   |
| `NotificacionsService.cs`      | ~10 líneas de cambio   | 🟠 Media  |
| `CobranzasService.cs`          | ~5 líneas de cambio    | 🟠 Media  |
| `PaymentDescriptionHelper.cs`  | Refactorizar completo  | 🟠 Media  |
| `PagoCuotaValidator.cs`        | ~4 líneas de cambio    | 🟠 Media  |
| `SociosRepository.cs`          | ~30 líneas de cambio   | 🔴 Alta   |
| `CuotaRepository.cs`           | ~10 líneas de cambio   | 🟠 Media  |
| `AnaliticasRepository.cs`      | ~5 líneas de cambio    | 🟡 Baja   |
| `PaymentTokenRepository.cs`    | Sin cambios directos   | ✅ N/A    |
| `AppDbcontext.cs`              | Agregar DbSet + config | 🟠 Media  |
| **NUEVOS archivos**            | 4-5 archivos nuevos    | 🔴 Alta   |

---

## ⚠️ Consideraciones Importantes

### 1. Migración de datos existentes

Los datos existentes usaban `Semestre = 1` o `Semestre = 2`. Al renombrar a `NumeroPeriodo`, los valores siguen siendo válidos si la configuración por defecto es Semestral. **No se pierden datos**.

### 2. Frontend

El frontend también referencia `semestre` en DTOs y vistas. Se necesitará:

- Renombrar las propiedades en los modelos del frontend
- Adaptar labels y textos para mostrar el nombre del período dinámicamente
- El endpoint `GET /api/configuracion/periodos` servirá para que el frontend sepa qué tipo de período mostrar

### 3. Cron Jobs

Los jobs `CreatePaymentTokensJob` y `SendWhatsappPaymentNotificacionJob` no necesitan cambios directos ya que llaman a servicios que sí serán refactorizados. Los cambios se propagan automáticamente.

### 4. Testing

Se recomienda crear tests unitarios para `PeriodoCalculator` con todos los tipos de período:

- Validar que `ObtenerNumeroPeriodo(1)` devuelve 1 para cualquier tipo
- Validar que `GenerarPeriodosDesdeAsociacion()` genera la cantidad correcta
- Validar bordes (diciembre, cambio de año, etc.)

### 5. Orden de ejecución recomendado

1. **Fase 1** primero (crear las abstracciones)
2. **Fase 6** con rename de columnas en la misma migración
3. **Fase 2** para adaptar los modelos
4. **Fases 3-5** de forma paralela o secuencial según preferencia
5. **Fase 7** al final para conectar todo
6. **Fase 8** cuando sea necesario el panel de configuración

---

## 🎯 Resultado Esperado

Después de esta refactorización:

- ✅ Un administrador SaaS puede configurar periodos **mensuales, bimestrales, trimestrales, cuatrimestrales, semestrales o anuales**
- ✅ Toda la lógica de cálculo está centralizada en `PeriodoCalculator`
- ✅ Los servicios y repositorios son **agnósticos al tipo de período**
- ✅ No hay `Month <= 6 ? 1 : 2` en ningún lugar del código
- ✅ Las migraciones de datos existentes son transparentes
- ✅ Los cron jobs se adaptan automáticamente
- ✅ Los avisos de vencimiento usan la configuración dinámica

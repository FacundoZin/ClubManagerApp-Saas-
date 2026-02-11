# Query Filters en Entity Framework Core

## 📋 Descripción General

Los **Query Filters** son filtros globales aplicados automáticamente a todas las consultas de Entity Framework Core para determinadas entidades. Esto permite implementar patrones como _soft delete_, filtrado por tenant, o filtrado de datos históricos sin necesidad de especificar las condiciones en cada consulta.

## 🔍 Filtros Implementados

### 1. Socio - Filtro de Socios Activos

**Ubicación:** [AppDbContext.cs:50](file:///d:/Repositorio/SistemaClubAbuelos/APIClub/Infrastructure/Persistence/Data/AppDbcontext.cs#L50)

```csharp
entity.HasQueryFilter(s => s.IsActivo);
```

**Propósito:** Implementa un patrón de _soft delete_ para socios. Solo se recuperan socios activos en las consultas normales.

**Comportamiento:**

- ✅ Las consultas normales solo retornan socios con `IsActivo = true`
- ❌ Los socios con `IsActivo = false` están ocultos por defecto
- 🔓 Se puede desactivar usando `.IgnoreQueryFilters()`

**Ejemplo de uso:**

```csharp
// Solo socios activos (filtro aplicado automáticamente)
var sociosActivos = await _context.Socios.ToListAsync();

// Todos los socios, incluyendo inactivos
var todosSocios = await _context.Socios
    .IgnoreQueryFilters()
    .ToListAsync();
```

---

### 2. ReservaSalon - Filtro de Reservas Vigentes

**Ubicación:** [AppDbContext.cs:104](file:///d:/Repositorio/SistemaClubAbuelos/APIClub/Infrastructure/Persistence/Data/AppDbcontext.cs#L104)

```csharp
entity.HasQueryFilter(r => !r.IsCancelled && r.FechaAlquiler >= DateOnly.FromDateTime(DateTime.Today));
```

**Propósito:** Filtra reservas de salones para mostrar solo las vigentes (no canceladas y con fecha futura o actual).

**Comportamiento:**

- ✅ Solo retorna reservas con `IsCancelled = false`
- ✅ Solo retorna reservas con `FechaAlquiler >= hoy`
- ❌ Oculta reservas canceladas y pasadas
- 🔓 Se puede desactivar usando `.IgnoreQueryFilters()`

**Ejemplo de uso:**

```csharp
// Solo reservas vigentes (no canceladas y futuras/actuales)
var reservasVigentes = await _context.ReservasSalones.ToListAsync();

// Todas las reservas, incluyendo canceladas e históricas
var todasReservas = await _context.ReservasSalones
    .IgnoreQueryFilters()
    .ToListAsync();

// Reservas históricas (pasadas) no canceladas
var reservasPasadas = await _context.ReservasSalones
    .IgnoreQueryFilters()
    .Where(r => !r.IsCancelled && r.FechaAlquiler < DateOnly.FromDateTime(DateTime.Today))
    .ToListAsync();
```

---

### 3. Alquiler - Filtro de Alquileres Activos

**Ubicación:** [AppDbContext.cs:159](file:///d:/Repositorio/SistemaClubAbuelos/APIClub/Infrastructure/Persistence/Data/AppDbcontext.cs#L159)

```csharp
entity.HasQueryFilter(a => !a.Finalizado);
```

**Propósito:** Filtra alquileres de artículos para mostrar solo los activos (no finalizados).

**Comportamiento:**

- ✅ Solo retorna alquileres con `Finalizado = false`
- ❌ Oculta alquileres finalizados
- 🔓 Se puede desactivar usando `.IgnoreQueryFilters()`

**Ejemplo de uso:**

```csharp
// Solo alquileres activos (filtro aplicado automáticamente)
var alquileresActivos = await _context.alquileresArticulos
    .Include(a => a.Items)
    .ToListAsync();

// Todos los alquileres, incluyendo finalizados
var todosAlquileres = await _context.alquileresArticulos
    .IgnoreQueryFilters()
    .Include(a => a.Items)
    .ToListAsync();

// Solo alquileres finalizados
var alquileresFinalizados = await _context.alquileresArticulos
    .IgnoreQueryFilters()
    .Where(a => a.Finalizado)
    .ToListAsync();
```

---

## 🛠️ Cómo Desactivar Query Filters

### IMPORTANTE: Desactivar en las consulta específicas, no en el DbContext. (actualmente ignoramos los filtros en el repositorio de analiticas de la asociación)

```csharp
var resultado = await _context.Socios
    .IgnoreQueryFilters()
    .Where(s => s.Dni == "12345678")
    .FirstOrDefaultAsync();
```

---

## 📚 Recursos Adicionales

- [Documentación oficial de EF Core - Query Filters](https://learn.microsoft.com/en-us/ef/core/querying/filters)
- [AppDbContext.cs](file:///d:/Repositorio/SistemaClubAbuelos/APIClub/Infrastructure/Persistence/Data/AppDbcontext.cs) - Implementación completa

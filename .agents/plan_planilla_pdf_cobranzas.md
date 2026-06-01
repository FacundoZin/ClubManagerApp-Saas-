# Plan de Implementación: Descarga de Planilla PDF de Socios Deudores por Lote

## Objetivo

Agregar al módulo de cobranzas la funcionalidad de **descargar una planilla en PDF** que contenga **todos los socios deudores de un lote**. El PDF se genera en el backend usando **QuestPDF** y se devuelve como archivo al frontend para descarga directa. Internamente se utiliza **paginación** para no sobrecargar la memoria del servidor.

---

## Decisiones Técnicas

### Librería PDF: QuestPDF
- Paquete NuGet gratuito (community license para proyectos con ingresos < USD 1M)
- API fluida en C# puro, sin templates HTML ni dependencias externas
- Soporte nativo para tablas, paginación automática de contenido y layout responsive

### Paginación Interna
La generación del PDF **itera internamente** sobre el repositorio existente `GetSociosDeudoresByLote()` que ya tiene paginación. Se traen los datos en páginas (ej. de 50 socios) y se van agregando filas a la tabla del PDF hasta procesar todos los deudores. QuestPDF se encarga automáticamente de crear nuevas páginas del PDF cuando la tabla excede el espacio disponible.

### Monto Total a Pagar
Para cada socio se calcula: `cantidadPeriodosAdeudados × valorActualCuota`. El valor de cuota se obtiene una sola vez al inicio usando `ICuotaRepository.ObtenerValorCuota()`.

---

## Estructura de la Plantilla PDF

### Concepto General
La planilla NO es una lista de lectura para el cobrador, sino una **hoja de cupones recortables**. Cada cupón corresponde a un socio y será recortado por el cobrador para entregárselo al socio como comprobante/aviso de deuda. Los cupones se apilan verticalmente en la página, separados por **líneas de corte punteadas (✂)**, y QuestPDF crea nuevas páginas automáticamente cuando no hay más espacio.

### Layout de la Página

```
┌══════════════════════════════════════════════════════════════════════┐
│                    ENCABEZADO DE PÁGINA (se repite)                  │
│                                                                      │
│   ASOCIACIÓN CIVIL CASA DEL JUBILADO                                │
│   Planilla de Cobranza — Lote: [NombreLote]                        │
│   Zona: [CalleNorte] / [CalleSur] / [CalleEste] / [CalleOeste]     │
│   Fecha: dd/MM/yyyy  |  Valor cuota: $X.XXX,XX                     │
│                                                                      │
╞══════════════════════════════════════════════════════════════════════╡
│                                                                      │
│  ┌────────────────────────────────────────────────────────────────┐  │
│  │  ASOCIACIÓN CIVIL CASA DEL JUBILADO                           │  │
│  │  ─────────────────────────────────────────────────────────    │  │
│  │                                                                │  │
│  │  Socio: García, Juan                     DNI: 12.345.678      │  │
│  │  Dirección: Calle Falsa 123              Tel: 3534-123456     │  │
│  │                                                                │  │
│  │  Períodos adeudados: 2025-S1, 2025-S2                        │  │
│  │                                                                │  │
│  │  ┌──────────────────────────────────────────────────────────┐ │  │
│  │  │  TOTAL A PAGAR:  $XX.XXX,XX   (2 cuotas × $X.XXX,XX)   │ │  │
│  │  └──────────────────────────────────────────────────────────┘ │  │
│  │                                                                │  │
│  │  Firma cobrador: _______________  Firma socio: _____________  │  │
│  │  Fecha de cobro: ___/___/______                               │  │
│  └────────────────────────────────────────────────────────────────┘  │
│                                                                      │
│  ✂ · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · ✂   │
│                                                                      │
│  ┌────────────────────────────────────────────────────────────────┐  │
│  │  ASOCIACIÓN CIVIL CASA DEL JUBILADO                           │  │
│  │  ─────────────────────────────────────────────────────────    │  │
│  │                                                                │  │
│  │  Socio: López, María                    DNI: 87.654.321      │  │
│  │  Dirección: Av. Siempre Viva 456        Tel: 3534-567890     │  │
│  │                                                                │  │
│  │  Períodos adeudados: 2024-S2, 2025-S1, 2025-S2              │  │
│  │                                                                │  │
│  │  ┌──────────────────────────────────────────────────────────┐ │  │
│  │  │  TOTAL A PAGAR:  $XX.XXX,XX   (3 cuotas × $X.XXX,XX)   │ │  │
│  │  └──────────────────────────────────────────────────────────┘ │  │
│  │                                                                │  │
│  │  Firma cobrador: _______________  Firma socio: _____________  │  │
│  │  Fecha de cobro: ___/___/______                               │  │
│  └────────────────────────────────────────────────────────────────┘  │
│                                                                      │
│  ✂ · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · ✂   │
│                                                                      │
│  ┌─────────────────────────────────────────────────────────────── ┐  │
│  │  (siguiente cupón... se apilan hasta llenar la página,        │  │
│  │   luego QuestPDF crea una nueva página automáticamente)       │  │
│  └────────────────────────────────────────────────────────────────┘  │
│                                                                      │
│                                                         Página X / N │
└══════════════════════════════════════════════════════════════════════┘
```

### Detalle de cada Cupón
Cada cupón es una unidad independiente que contiene:

| Elemento | Descripción |
|----------|-------------|
| **Título institucional** | "ASOCIACIÓN CIVIL CASA DEL JUBILADO" — identifica la organización |
| **Datos del socio** | Apellido y Nombre, DNI, Dirección, Teléfono |
| **Períodos adeudados** | Lista de períodos (ej: "2024-S2, 2025-S1, 2025-S2") |
| **Total a pagar** | Monto destacado: `cantidadPeriodos × valorCuota` + detalle del cálculo |
| **Firma cobrador** | Línea en blanco para firma del cobrador |
| **Firma socio** | Línea en blanco para firma del socio al momento del pago |
| **Fecha de cobro** | Espacio en blanco para anotar la fecha real de cobro |

### Notas de implementación:
- **Encabezado de página**: Se repite en cada página (datos del lote, fecha generación, valor cuota). Es informativo para el cobrador, no se recorta
- **Cupones**: Se apilan verticalmente. Cada cupón tiene un **borde sólido** (rectángulo) y están separados por **líneas de corte punteadas con ✂**
- **Cantidad por página**: Aproximadamente 3-4 cupones por página A4, dependiendo del contenido de períodos adeudados
- **Salto de página automático**: QuestPDF detecta que un cupón no entra completo y lo mueve a la siguiente página (no se corta un cupón a la mitad)
- **Pie de página**: Número de página en cada página

---

## Cambios Propuestos

### 1. NuGet Package

#### [MODIFY] `APIClub/APIClub.csproj`

Agregar:
```xml
<PackageReference Include="QuestPDF" Version="2024.12.2" />
```

---

### 2. Interfaz del Servicio PDF (nueva)

#### [NEW] `APIClub/Domain/ModuloGestionCobradores/UseCases/IPdfPlanillaCobranzaService.cs`

```csharp
using APIClub.Application.Dtos.Lote;
using APIClub.Application.Dtos.Socios;

namespace APIClub.Domain.ModuloGestionCobradores.UseCases
{
    public interface IPdfPlanillaCobranzaService
    {
        byte[] GenerarPlanillaDeudores(
            PreviewLote lote, 
            List<PreviewSocioForCobranzaDto> socios, 
            decimal valorCuota);
    }
}
```

---

### 3. Implementación del Servicio PDF (nueva)

#### [NEW] `APIClub/Application/Services/PdfPlanillaCobranzaService.cs`

Responsabilidades:
- Recibe la lista completa de socios (ya acumulada por el orquestador), los datos del lote y el valor de cuota
- Usa QuestPDF para armar el documento con la estructura definida arriba
- Calcula `montoDeuda = socio.PeriodosAdeudados.Count * valorCuota` para cada socio
- Devuelve `byte[]` del PDF generado

Pseudocódigo del layout QuestPDF:
```csharp
public byte[] GenerarPlanillaDeudores(PreviewLote lote, List<PreviewSocioForCobranzaDto> socios, decimal valorCuota)
{
    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1.5f, Unit.Centimetre);

            // HEADER (se repite en cada página)
            page.Header().Element(header => {
                // Encabezado de la planilla: Institución, Lote, Zona, Fecha, Valor cuota
            });

            // CONTENT - Lista de Cupones apilados verticalmente
            page.Content().Element(content => {
                content.Column(column => {
                    foreach(var socio in socios) {
                        // ShowEntire() previene que un cupón se corte por la mitad en el salto de página
                        column.Item().ShowEntire().Element(cup => {
                            // Borde de cupón, Título institucional
                            // Datos: Apellido, Nombre, DNI, Teléfono, Dirección
                            // Períodos Adeudados y Total a Pagar
                            // Espacio para Firma Cobrador, Firma Socio y Fecha
                        });
                        
                        // Agregar línea de corte (ej. punteada)
                        column.Item().PaddingVertical(10).LineHorizontal(1).LinePattern(LinePattern.Dashed);
                    }
                });
            });

            // FOOTER (se repite en cada página)
            page.Footer().Element(footer => {
                // Número de página: "Página X / N"
            });
        });
    });

    return document.GeneratePdf();
}
```

---

### 4. Interfaz del Servicio de Cobranzas

#### [MODIFY] `APIClub/Domain/ModuloGestionCobradores/UseCases/ICobranzasServices.cs`

Agregar:
```csharp
Task<Result<byte[]>> GenerarPlanillaCobranzasPdf(int idLote);
```

---

### 5. Servicio de Cobranzas (orquestación con paginación interna)

#### [MODIFY] `APIClub/Application/Services/CobranzasService.cs`

**Cambios:**
1. Inyectar `ICuotaRepository` y `IPdfPlanillaCobranzaService` en el constructor
2. Implementar `GenerarPlanillaCobranzasPdf()`:

```csharp
public async Task<Result<byte[]>> GenerarPlanillaCobranzasPdf(int idLote)
{
    // 1. Obtener datos del lote
    var lote = await _context.Lotes.FindAsync(idLote);
    if (lote == null) 
        return Result<byte[]>.NotFound("Lote no encontrado");

    // 2. Obtener valor actual de cuota
    var valorCuota = await _cuotaRepository.ObtenerValorCuota();

    // 3. Iterar con paginación para acumular todos los deudores
    var hoy = DateTime.Now;
    int anioActual = hoy.Year;
    int semestreActual = hoy.Month <= 6 ? 1 : 2;

    var todosLosSocios = new List<PreviewSocioForCobranzaDto>();
    int pageNumber = 1;
    int pageSize = 50; // tamaño de página interno

    while (true)
    {
        var (items, totalCount) = await _SociosRepository
            .GetSociosDeudoresByLote(idLote, anioActual, semestreActual, pageNumber, pageSize);
        
        todosLosSocios.AddRange(items);

        if (todosLosSocios.Count >= totalCount) break;
        pageNumber++;
    }

    if (todosLosSocios.Count == 0)
        return Result<byte[]>.Error("No hay socios deudores en este lote", 404);

    // 4. Armar DTO del lote para el PDF
    var lotePreview = new PreviewLote
    {
        Id = lote.Id,
        NombreLote = lote.NombreLote,
        CalleNorte = lote.CalleNorte,
        CalleSur = lote.CalleSur,
        CalleEste = lote.CalleEste,
        CalleOeste = lote.CalleOeste
    };

    // 5. Generar PDF
    var pdfBytes = _pdfService.GenerarPlanillaDeudores(lotePreview, todosLosSocios, valorCuota);

    return Result<byte[]>.Exito(pdfBytes);
}
```

---

### 6. Controller (nuevo endpoint)

#### [MODIFY] `APIClub/Contrrollers/CobranzasController.cs`

Agregar:
```csharp
[HttpGet("lotes/{IdLote}/planilla-deudores")]
public async Task<IActionResult> GetPlanillaCobranzas(int IdLote)
{
    var result = await _cobranzasServices.GenerarPlanillaCobranzasPdf(IdLote);

    if (!result.Exit) return StatusCode(result.Errorcode, result.Errormessage);

    return File(result.Data, "application/pdf", $"planilla_deudores_lote_{IdLote}.pdf");
}
```

---

### 7. Registro en DI

#### [MODIFY] `APIClub/Program.cs`

Agregar:
```csharp
// Al inicio del archivo o en la configuración
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// En el registro de servicios
builder.Services.AddScoped<IPdfPlanillaCobranzaService, PdfPlanillaCobranzaService>();
```

---

### 8. Frontend - Servicio HTTP

#### [MODIFY] `frontend/src/services/CobranzasService.js`

Agregar nuevo método:
```javascript
async descargarPlanillaDeudores(idLote) {
    const response = await fetch(
        `${API_URL}/lotes/${idLote}/planilla-deudores`,
        { credentials: 'include' }
    );

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Error al descargar la planilla');
    }

    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `planilla_deudores_lote_${idLote}.pdf`;
    document.body.appendChild(a);
    a.click();
    a.remove();
    window.URL.revokeObjectURL(url);
}
```

---

### 9. Frontend - UI (botón de descarga)

#### [MODIFY] `frontend/src/components/ModuloCobradores/BuscarSociosTab/BuscarSociosTab.vue`

Agregar un **botón "Descargar Planilla PDF"** visible cuando hay un lote seleccionado y socios cargados. Ubicación: en la barra superior, junto al selector de lote. Incluye:
- Ícono de descarga
- Estado de loading (spinner) durante la generación/descarga
- Manejo de errores con toast/alert

---

## Resumen de Archivos

| Archivo | Acción | Descripción |
|---------|--------|-------------|
| `APIClub.csproj` | MODIFY | Agregar paquete QuestPDF |
| `IPdfPlanillaCobranzaService.cs` | **NEW** | Interfaz del servicio generador de PDF |
| `PdfPlanillaCobranzaService.cs` | **NEW** | Implementación: layout y generación del PDF con QuestPDF |
| `ICobranzasServices.cs` | MODIFY | Agregar firma `GenerarPlanillaCobranzasPdf` |
| `CobranzasService.cs` | MODIFY | Orquestación: paginación interna + valor cuota + invocar generador PDF |
| `CobranzasController.cs` | MODIFY | Nuevo endpoint `GET lotes/{id}/planilla-deudores` |
| `Program.cs` | MODIFY | Registrar DI del servicio PDF + licencia QuestPDF |
| `CobranzasService.js` | MODIFY | Método frontend para descargar blob como PDF |
| `BuscarSociosTab.vue` | MODIFY | Botón "Descargar Planilla PDF" |

---

## Verificación

### Build
```bash
dotnet build
```

### Pruebas Manuales
1. Probar endpoint vía Swagger: `GET /api/Cobranzas/lotes/{IdLote}/planilla-deudores`
   - Con lote que tiene deudores → debe devolver PDF válido
   - Con lote sin deudores → debe devolver error 404 controlado
   - Con lote inexistente → debe devolver error 404
2. Abrir el PDF descargado y verificar:
   - Encabezado correcto (nombre asociación, datos del lote, fecha, valor cuota)
   - Tabla con todos los socios del lote
   - Períodos adeudados correctos por socio
   - Monto calculado correctamente (períodos × valor cuota)
   - Totales al final del documento
   - Paginación automática cuando hay muchos socios
3. Desde el frontend: verificar que el botón descarga correctamente el PDF

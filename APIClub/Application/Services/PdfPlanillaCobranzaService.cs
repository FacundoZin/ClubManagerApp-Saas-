using APIClub.Application.Dtos.Lote;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.ModuloGestionCobradores.UseCases;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace APIClub.Application.Services
{
    public class PdfPlanillaCobranzaService : IPdfPlanillaCobranzaService
    {
        public byte[] GenerarPlanillaDeudores(PreviewLote lote, List<PreviewSocioForCobranzaDto> socios, decimal valorCuota)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));

                    // ENCABEZADO DE LA PÁGINA (Se repite en cada página)
                    page.Header().Column(headerCol =>
                    {
                        headerCol.Item().Row(row =>
                        {
                            row.RelativeItem().Column(titleCol =>
                            {
                                titleCol.Item().Text("ASOCIACIÓN CIVIL CASA DEL JUBILADO")
                                    .Bold()
                                    .FontSize(15)
                                    .FontColor(Colors.Teal.Darken3);

                                titleCol.Item().Text($"Planilla de Cobranza — Lote: {lote.NombreLote}")
                                    .Bold()
                                    .FontSize(12)
                                    .FontColor(Colors.Grey.Darken2);
                            });

                            row.ConstantItem(120).AlignRight().Column(dateCol =>
                            {
                                dateCol.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}")
                                    .FontSize(9)
                                    .Italic()
                                    .FontColor(Colors.Grey.Darken1);

                                dateCol.Item().Text($"Valor Cuota: ${valorCuota:N2}")
                                    .Bold()
                                    .FontSize(10)
                                    .FontColor(Colors.Teal.Darken4);
                            });
                        });

                        // Zona/Límites del lote
                        var limites = uldLimites(lote);
                        if (!string.IsNullOrEmpty(limites))
                        {
                            headerCol.Item().PaddingTop(3).Text(t =>
                            {
                                t.Span("Zona: ").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                                t.Span(limites).FontSize(9).Italic().FontColor(Colors.Grey.Darken2);
                            });
                        }

                        headerCol.Item().PaddingVertical(5).LineHorizontal(1.5f).LineColor(Colors.Teal.Darken3);
                    });

                    // CONTENIDO: Cupones de pago recortables organizados en dos columnas
                    page.Content().PaddingTop(10).Grid(grid =>
                    {
                        grid.Columns(2); // 2 columnas por fila

                        foreach (var socio in socios)
                        {
                            // ShowEntire() evita que el cupón se divida a la mitad entre dos páginas
                            grid.Item().ShowEntire().Column(cupItem =>
                            {
                                // Contenedor del Cupón (Borde gris, fondo blanco)
                                cupItem.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.White).Padding(10).Column(cup =>
                                {
                                    // Encabezado del Cupón
                                    cup.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("CASA DEL JUBILADO")
                                            .Bold()
                                            .FontSize(9)
                                            .FontColor(Colors.Teal.Darken3);

                                        row.ConstantItem(60).AlignRight().Text("CUPÓN")
                                            .Bold()
                                            .FontSize(7.5f)
                                            .FontColor(Colors.Grey.Darken1);
                                    });

                                    cup.Item().PaddingVertical(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                                    // Datos del Socio apilados verticalmente para evitar que se apriete
                                    cup.Item().Text(t =>
                                    {
                                        t.Span("Socio: ").Bold().FontSize(8.5f);
                                        t.Span($"{socio.Apellido}, {socio.Nombre}").FontSize(8.5f);
                                    });

                                    cup.Item().PaddingTop(1).Text(t =>
                                    {
                                        t.Span("DNI: ").Bold().FontSize(8.5f);
                                        t.Span(socio.Dni ?? "—").FontSize(8.5f);
                                    });

                                    cup.Item().PaddingTop(1).Text(t =>
                                    {
                                        t.Span("Dirección: ").Bold().FontSize(8.5f);
                                        t.Span(socio.Direcccion ?? "No registrada").FontSize(8.5f);
                                    });

                                    cup.Item().PaddingTop(1).Text(t =>
                                    {
                                        t.Span("Teléfono: ").Bold().FontSize(8.5f);
                                        t.Span(socio.Telefono ?? "—").FontSize(8.5f);
                                    });

                                    // Períodos Adeudados
                                    var periodosAdeudadosStr = string.Join(", ", socio.PeriodosAdeudados.Select(p => $"{p.Anio}-S{p.Semestre}"));
                                    cup.Item().PaddingTop(3).Text(t =>
                                    {
                                        t.Span("Períodos: ").Bold().FontSize(8.5f);
                                        t.Span(periodosAdeudadosStr).FontSize(8.5f);
                                    });

                                    // Destacado: Total a Pagar
                                    var cantidadCuotas = socio.PeriodosAdeudados.Count;
                                    var totalAPagar = cantidadCuotas * valorCuota;

                                    cup.Item().PaddingTop(6).Background(Colors.Grey.Lighten3).Padding(6).Column(totCol =>
                                    {
                                        totCol.Item().Text(t =>
                                        {
                                            t.Span("TOTAL: ").Bold().FontSize(9).FontColor(Colors.Grey.Darken4);
                                            t.Span($" ${totalAPagar:N2}").Bold().FontSize(10).FontColor(Colors.Teal.Darken4);
                                        });

                                        totCol.Item().Text($"({cantidadCuotas} cuotas × ${valorCuota:N2})")
                                            .Italic()
                                            .FontSize(7.5f)
                                            .FontColor(Colors.Grey.Darken3);
                                    });
                                });
                            });
                        }
                    });

                    // PIE DE PÁGINA (Se repite en cada página)
                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().Text("Asociación Civil Casa del Jubilado — Planilla de Cobranza")
                            .FontSize(8)
                            .Italic()
                            .FontColor(Colors.Grey.Darken1);

                        row.RelativeItem().AlignRight().Text(x =>
                        {
                            x.Span("Página ").FontSize(8).FontColor(Colors.Grey.Darken1);
                            x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1);
                            x.Span(" de ").FontSize(8).FontColor(Colors.Grey.Darken1);
                            x.TotalPages().FontSize(8).FontColor(Colors.Grey.Darken1);
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        private string uldLimites(PreviewLote lote)
        {
            var partes = new List<string>();
            if (!string.IsNullOrWhiteSpace(lote.CalleNorte)) partes.Add($"Norte: {lote.CalleNorte.Trim()}");
            if (!string.IsNullOrWhiteSpace(lote.CalleSur)) partes.Add($"Sur: {lote.CalleSur.Trim()}");
            if (!string.IsNullOrWhiteSpace(lote.CalleEste)) partes.Add($"Este: {lote.CalleEste.Trim()}");
            if (!string.IsNullOrWhiteSpace(lote.CalleOeste)) partes.Add($"Oeste: {lote.CalleOeste.Trim()}");
            return string.Join(" | ", partes);
        }
    }
}

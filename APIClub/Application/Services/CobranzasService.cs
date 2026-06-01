using APIClub.Application.Common;
using APIClub.Application.Dtos.Cobrador;
using APIClub.Application.Dtos.Lote;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.Auth.Repositories;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.ModuloGestionCobradores.Models;
using APIClub.Domain.ModuloGestionCobradores.Repositorios;
using APIClub.Domain.ModuloGestionCobradores.UseCases;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Application.Services
{
    public class CobranzasService : ICobranzasServices
    {
        private readonly ISocioRepository _SociosRepository;
        private readonly IUsuariosRepository _UsuariosRepository;
        private readonly AppDbcontext _context;
        private readonly IHistorialCobradoresRepository _historialCobradoresRepository;
        private readonly ICuotaRepository _cuotaRepository;
        private readonly IPdfPlanillaCobranzaService _pdfService;

        public CobranzasService(ISocioRepository sociosRepository, AppDbcontext context, IUsuariosRepository usuariosRepository,
            IHistorialCobradoresRepository historialCobradoresRepository, ICuotaRepository cuotaRepository, IPdfPlanillaCobranzaService pdfService)
        {
            _SociosRepository = sociosRepository;
            _context = context;
            _UsuariosRepository = usuariosRepository;
            _historialCobradoresRepository = historialCobradoresRepository;
            _cuotaRepository = cuotaRepository;
            _pdfService = pdfService;
        }

        public async Task<List<PreviewLote>> GetLotesPreview()
        {
            var lotes = await _context.Lotes.ToListAsync();

            return lotes.Select(s => new PreviewLote
            {
                Id = s.Id,
                NombreLote = s.NombreLote,
                CalleNorte = s.CalleNorte,
                CalleSur = s.CalleSur,
                CalleEste = s.CalleEste,
                CalleOeste = s.CalleOeste,
            }).ToList();
        }

        public async Task<Result<PagedResult<PreviewSocioForCobranzaDto>>> ListarSociosDedudoresPorLote(int Idlote, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var hoy = DateTime.Now;
                int anioActual = hoy.Year;
                int semestreActual = hoy.Month <= 6 ? 1 : 2;

                var (dto, totalCount) = await _SociosRepository.GetSociosDeudoresByLote(Idlote, anioActual, semestreActual, pageNumber, pageSize);

                var pagedResult = new PagedResult<PreviewSocioForCobranzaDto>(dto, totalCount, pageNumber, pageSize);

                return Result<PagedResult<PreviewSocioForCobranzaDto>>.Exito(pagedResult);

            }
            catch (Exception)
            {
                return Result<PagedResult<PreviewSocioForCobranzaDto>>.Error("algo salio mal al obtener los socios", 500);
            }

        }

        public async Task<Result<bool>> CrearLote(CreateLoteDto dto)
        {
            try
            {
                var nuevoLote = new Lote
                {
                    NombreLote = dto.NombreLote,
                    CalleNorte = dto.CalleNorte,
                    CalleSur = dto.CalleSur,
                    CalleEste = dto.CalleEste,
                    CalleOeste = dto.CalleOeste
                };

                _context.Lotes.Add(nuevoLote);
                await _context.SaveChangesAsync();

                return Result<bool>.Exito(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Error("Error al crear el lote: " + ex.Message, 500);
            }
        }

        public async Task<List<CobradorDto>> GetListaCobradores()
        {
            var cobradores = await _UsuariosRepository.GetUsuariosCobradores();

            var dto = cobradores.Select(c => new CobradorDto
            {
                idCobrador = c.Id,
                NombreCompleto = c.NombreUsuario,
            }).ToList();

            return dto;
        }

        public async Task<HistorialCobradorDto> GetHistorialCobradorByMes(int idCobrador, int mes, int anio)
        {
            var historial = await _historialCobradoresRepository.GetHistorialCobradorByMes(idCobrador, mes, anio);

            var cobrosRealizados = historial.Select(c => new CobroDto
            {
                FechaCobro = c.FechaCobro,
                MontoCobrado = c.MontoCobrado,
                NombreSocio = c.NombreSocio
            })
                .ToList();

            var montoTotalCobrado = cobrosRealizados.Sum(c => c.MontoCobrado);

            return new HistorialCobradorDto { Anio = anio, Mes = mes, CobrosRealizados = cobrosRealizados, MontoTotalCobrado = montoTotalCobrado };
        }

        public async Task<Result<byte[]>> GenerarPlanillaCobranzasPdf(int idLote)
        {
            try
            {
                // 1. Obtener datos del lote
                var lote = await _context.Lotes.FindAsync(idLote);
                if (lote == null)
                    return Result<byte[]>.Error("Lote no encontrado", 404);

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
                    
                    if (items != null && items.Count > 0)
                    {
                        todosLosSocios.AddRange(items);
                    }

                    if (todosLosSocios.Count >= totalCount || items == null || items.Count == 0)
                        break;

                    pageNumber++;
                }

                if (todosLosSocios.Count == 0)
                    return Result<byte[]>.Error("No hay socios deudores en este lote", 404);

                // 4. Armar DTO del lote para el PDF
                var lotePreview = new PreviewLote
                {
                    Id = lote.Id,
                    NombreLote = lote.NombreLote,
                    CalleNorte = lote.CalleNorte ?? string.Empty,
                    CalleSur = lote.CalleSur ?? string.Empty,
                    CalleEste = lote.CalleEste ?? string.Empty,
                    CalleOeste = lote.CalleOeste ?? string.Empty
                };

                // 5. Generar PDF
                var pdfBytes = _pdfService.GenerarPlanillaDeudores(lotePreview, todosLosSocios, valorCuota);

                return Result<byte[]>.Exito(pdfBytes);
            }
            catch (Exception ex)
            {
                return Result<byte[]>.Error($"Error al generar la planilla PDF: {ex.Message}", 500);
            }
        }
    }
}

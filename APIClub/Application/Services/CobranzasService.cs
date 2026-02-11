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
        public CobranzasService(ISocioRepository sociosRepository, AppDbcontext context, IUsuariosRepository usuariosRepository,
            IHistorialCobradoresRepository historialCobradoresRepository)
        {
            _SociosRepository = sociosRepository;
            _context = context;
            _UsuariosRepository = usuariosRepository;
            _historialCobradoresRepository = historialCobradoresRepository;
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
    }
}

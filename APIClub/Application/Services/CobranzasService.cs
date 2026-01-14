using APIClub.Application.Common;
using APIClub.Application.Dtos.Lote;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.GestionSocios;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Application.Services
{
    public class CobranzasService : ICobranzasServices
    {
        private readonly ISocioRepository _SociosRepository;
        private readonly AppDbcontext _context;

        public CobranzasService(ISocioRepository sociosRepository, AppDbcontext context)
        {
            _SociosRepository = sociosRepository;
            _context = context;
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

                var (paginatedSocios, totalCount) = await _SociosRepository.GetSociosDeudoresByLote(Idlote, anioActual, semestreActual, pageNumber, pageSize);

                var dto = paginatedSocios.Select(s => new PreviewSocioForCobranzaDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Apellido = s.Apellido,
                    Dni = s.Dni,
                    Telefono = s.Telefono,
                    Direcccion = s.Direcccion,

                }).ToList();

                var pagedResult = new PagedResult<PreviewSocioForCobranzaDto>(dto, totalCount, pageNumber, pageSize);

                return Result<PagedResult<PreviewSocioForCobranzaDto>>.Exito(pagedResult);

            }
            catch (Exception ex)
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
    }
}

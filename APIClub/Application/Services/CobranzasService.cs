using APIClub.Application.Common;
using APIClub.Application.Dtos.Lote;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.GestionSocios;
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
                Calle1 = s.Calle1,
                Calle2 = s.Calle2,
                Calle3 = s.Calle3,
                Calle4 = s.Calle4,
            }).ToList();
        }

        public async Task<Result<List<PreviewSocioForCobranzaDto>>> ListarSociosDedudoresPorLote(int Idlote)
        {
            try
            {
                var hoy = DateTime.Now;
                int anioActual = hoy.Year;
                int semestreActual = hoy.Month <= 6 ? 1 : 2;

                var socios = await _SociosRepository.GetSociosDeudoresByLote(Idlote, anioActual, semestreActual);

                var dto = socios.Select(s => new PreviewSocioForCobranzaDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Apellido = s.Apellido,
                    Dni = s.Dni,
                    Telefono = s.Telefono,
                    Direcccion = s.Direcccion,

                }).ToList();

                return Result<List<PreviewSocioForCobranzaDto>>.Exito(dto);

            }
            catch (Exception ex)
            {
                return Result<List<PreviewSocioForCobranzaDto>>.Error("algo salio mal al obtener los socios", 500);
            }
            
        }
    }
}

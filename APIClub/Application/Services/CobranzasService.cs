using APIClub.Application.Common;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.GestionSocios;
using APIClub.Domain.GestionSocios.Repositories;

namespace APIClub.Application.Services
{
    public class CobranzasService : ICobranzasServices
    {
        private readonly ISocioRepository _SociosRepository;

        public CobranzasService(ISocioRepository sociosRepository)
        {
            _SociosRepository = sociosRepository;
        }

        public async Task<Result<List<PreviewSocioForCobranzaDto>>> ListarSociosDedudoresPorLote(string lote)
        {
            try
            {
                var hoy = DateTime.Now;
                int anioActual = hoy.Year;
                int semestreActual = hoy.Month <= 6 ? 1 : 2;

                var socios = await _SociosRepository.GetSociosDeudoresByLote(lote, anioActual, semestreActual);

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

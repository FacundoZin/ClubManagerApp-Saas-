using APIClub.Common;
using APIClub.Dtos.Socios;
using APIClub.Interfaces.Repository;
using APIClub.Interfaces.Services;
using APIClub.Models;

namespace APIClub.Services
{
    public class SocioService : ISocioService
    {
        private readonly ISocioRepository _SocioRepository;
        public SocioService(ISocioRepository socioRepository)
        {
            _SocioRepository = socioRepository;
        }


        public async Task<Result<CreatedSocio>> cargarSocio(CreateSocioDto _dto)
        {
            var exists = await _SocioRepository.SocioExists(_dto.Dni);

            if (exists)
            {
                return Result<CreatedSocio>.Error("el socio que quiere cargar ya fue cargado", 400);
            }

            var socio = new Socio
            {
                Nombre = _dto.Nombre,
                Apellido = _dto.Apellido,
                Dni = _dto.Dni,
                Telefono = _dto.Telefono,
                Direcccion = _dto.Direcccion,
                Lote = _dto.Lote,
                Localidad = _dto.Localidad
            };

            await _SocioRepository.cargarSocio(socio);

            return Result<CreatedSocio>.Exito(new CreatedSocio
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
            });
        }
    }
}

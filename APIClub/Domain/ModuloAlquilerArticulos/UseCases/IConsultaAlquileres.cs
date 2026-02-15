using APIClub.Application.Common;
using APIClub.Application.Dtos.AlquilerDeArticulos;

namespace APIClub.Domain.AlquilerArticulos.UseCases
{
    public interface IConsultaAlquileres
    {
        Task<Result<AlquilerDto?>> GetAlquilerById(int id);
        Task<Result<AlquilerPreviewDto?>> GetAlquilerBySocio(string socioDni);
        Task<Result<PagedResult<AlquilerPreviewDto>>> GetAlquileresActivos(int pageNumber, int pageSize);
        Task<Result<StatusAlquileresSociosDto>> ObtenerEstadoAlquilerSocio(string dni);
    }
}

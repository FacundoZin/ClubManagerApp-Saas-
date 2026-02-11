using APIClub.Application.Common;
using APIClub.Application.Dtos.Articulos;

namespace APIClub.Domain.AlquilerArticulos.UseCases
{
    public interface IManagmentArticulosService
    {
        Task<Result<ArticuloDto>> CargarArticulo(CreateArticuloDto dto);
        Task<Result<List<ArticuloDto>>> GetAllArticulos();
        Task<Result<ArticuloDto>> UpdatePrecioArticulo(int id, int nuevoPrecio);
    }
}

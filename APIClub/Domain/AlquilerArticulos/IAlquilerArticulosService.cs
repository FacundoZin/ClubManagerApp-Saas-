using APIClub.Common;
using APIClub.Dtos.Articulos;

namespace APIClub.Domain.AlquilerArticulos
{
    public interface IAlquilerArticulosService
    {
        Task<Result<ArticuloDto>> CargarArticulo(CreateArticuloDto dto);
        Task<Result<List<ArticuloDto>>> GetAllArticulos();
        Task<Result<ArticuloDto>> UpdatePrecioArticulo(int id, int nuevoPrecio);
    }
}

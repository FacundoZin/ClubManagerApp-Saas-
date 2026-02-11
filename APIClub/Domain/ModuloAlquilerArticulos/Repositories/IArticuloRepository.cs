using APIClub.Domain.AlquilerArticulos.Models;

namespace APIClub.Domain.AlquilerArticulos.Repositories
{
    public interface IArticuloRepository
    {
        Task CargarArticulo(Articulo articulo);
        Task<List<Articulo>> GetAllArticulos();
        Task<Articulo?> GetArticuloById(int id);
        Task UpdateArticulo(Articulo articulo);
        Task<bool> ArticuloExistsByNombre(string nombre);
    }
}

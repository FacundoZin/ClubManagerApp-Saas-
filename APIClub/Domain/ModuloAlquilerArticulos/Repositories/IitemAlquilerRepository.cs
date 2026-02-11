using APIClub.Domain.AlquilerArticulos.Models;

namespace APIClub.Domain.AlquilerArticulos.Repositories
{
    public interface IitemAlquilerRepository
    {
        Task<ItemAlquiler?> GetItemAlquilerById(int id);
        Task<bool> UpdateItemAlquiler(ItemAlquiler alquilerArticulo);
        Task<bool> RemoveItemAlquiler(ItemAlquiler alquilerArticulo);
    }
}

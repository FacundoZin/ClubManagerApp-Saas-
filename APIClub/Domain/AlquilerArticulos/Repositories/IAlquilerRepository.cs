using APIClub.Domain.AlquilerArticulos.Models;
using APIClub.Dtos.AlquilerDeArticulos;

namespace APIClub.Domain.AlquilerArticulos.Repositories
{
    public interface IAlquilerRepository
    {
        Task<Alquiler> CrearAlquiler(Alquiler alquiler);
        Task<Alquiler?> GetAlquilerById(int id);
        Task<Alquiler?> GetAlquilerByIdWithDetails(int id);
        Task<Alquiler?> GetAlquilerBySocio(int idSocio);
        Task<List<AlquilerPreviewDto>> GetAlquileresActivos();
        Task<bool> UpdateAlquiler(Alquiler alquiler);
        Task<PagoAlquilerDeArticulos> RegistrarPago(PagoAlquilerDeArticulos pago);
        Task<ItemAlquiler> AgregarItem(ItemAlquiler item);
        Task<int> CalcularMontoAlquiler(int alquilerId);
    }
}

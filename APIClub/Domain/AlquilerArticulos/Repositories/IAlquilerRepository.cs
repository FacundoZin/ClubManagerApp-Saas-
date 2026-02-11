using APIClub.Application.Dtos.AlquilerDeArticulos;
using APIClub.Domain.AlquilerArticulos.Models;

namespace APIClub.Domain.AlquilerArticulos.Repositories
{
    public interface IAlquilerRepository
    {
        Task<Alquiler> CrearAlquiler(Alquiler alquiler);
        Task<Alquiler?> GetAlquilerById(int id);
        Task<Alquiler?> GetAlquilerByIdWithDetails(int id);
        Task<Alquiler?> GetAlquilerBySocio(int idSocio);
        Task<(List<AlquilerPreviewDto> Items, int TotalCount)> GetAlquileresActivos(int pageNumber, int pageSize);
        Task<bool> UpdateAlquiler(Alquiler alquiler);
        Task<PagoAlquilerDeArticulos> RegistrarPago(PagoAlquilerDeArticulos pago);
        Task<ItemAlquiler> AgregarItem(ItemAlquiler item);
        Task<int> CalcularMontoAlquiler(int alquilerId);
        Task<bool> HasActiveAlquilerBySocio(int socioId);
        Task<(bool HasActiveAlquiler, int? AlquilerId)> TrySearchActiveAlquilerBySocio(int socioId);
    }
}

using APIClub.Application.Dtos.Viajes.Views;
using APIClub.Domain.ModuloGestionViajes.Models;

namespace APIClub.Domain.ModuloGestionViajes.Repositories
{
    public interface IViajeReadRepository
    {
        Task<Viaje?> GetViajeById(int id);
        Task<Viaje?> GetViajeByIdWithVariantes(int id);
        Task<FullViewViajeDto?> GetViajeCompleto(int id);
        Task<VarianteViaje?> GetVarianteById(int id);
        Task<List<Viaje>> ListarViajesDisponibles();
        Task<List<VarianteViaje>> ListarVariantesDeViaje(int idViajeBase);
        Task<bool> ViajeExists(int id);
        Task<bool> VarianteExists(int id);
    }
}

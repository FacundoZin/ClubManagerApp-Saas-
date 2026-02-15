using APIClub.Application.Dtos.Viajes.ComboBox;
using APIClub.Application.Dtos.Viajes.Views;
using APIClub.Domain.ModuloGestionViajes.Models;

namespace APIClub.Domain.ModuloGestionViajes.Repositories
{
    public interface IViajeReadRepository
    {
        Task<Viaje?> GetViajeById(int id);
        Task<Viaje?> GetViajeByIdWithVariantes(int id);
        Task<Viaje?> GetViajeCompleto(int id);
        Task<VarianteViaje?> GetVarianteById(int id);
        Task<List<Viaje>> ListarViajesDisponibles();
        Task<List<VarianteViaje>> ListarVariantesDeViaje(int idViajeBase);
        Task<InscriptoViaje?> GetInscriptoById(int id);
        Task<bool> ViajeExists(int id);
        Task<bool> VarianteExists(int id);
        Task<List<ComboBoxViajes>> GetComboBoxViajes();
        Task<List<ComboBoxVariantesViaje>> GetComboBoxVariantesDeViaje(int idViajeBase);
        Task<bool> EstaInscripto(int socioId, int varianteViajeId);
    }
}

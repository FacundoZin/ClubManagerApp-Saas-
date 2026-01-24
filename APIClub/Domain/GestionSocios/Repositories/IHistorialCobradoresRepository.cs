using APIClub.Domain.GestionSocios.Models;

namespace APIClub.Domain.GestionSocios.Repositories
{
    public interface IHistorialCobradoresRepository
    {
        void AddCobroToHistorial(RegistroCobrador cobro);
        Task<List<RegistroCobrador>> GetHistorialCobradorByMes(int idCobrador, int mes, int anio);
    }
}

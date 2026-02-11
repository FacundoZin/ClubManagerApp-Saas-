using APIClub.Domain.ModuloGestionCobradores.Models;

namespace APIClub.Domain.ModuloGestionCobradores.Repositorios
{
    public interface IHistorialCobradoresRepository
    {
        void AddCobroToHistorial(RegistroCobrador cobro);
        Task<List<RegistroCobrador>> GetHistorialCobradorByMes(int idCobrador, int mes, int anio);
    }
}

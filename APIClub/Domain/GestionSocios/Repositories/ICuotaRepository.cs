using APIClub.Domain.GestionSocios.Models;

namespace APIClub.Domain.GestionSocios.Repositories
{
    public interface ICuotaRepository
    {
        Task<decimal> ObtenerValorCuota();
        Task<decimal> ActualizarValorCuota(decimal valor, DateTime FechaActualizacion);
        void RegistrarCuotas(IEnumerable<Cuota> cuotas);
    }
}

namespace APIClub.Domain.GestionSocios.Repositories
{
    public interface ICuotaRepository
    {
        Task<decimal> ObtenerValorCuota();
        Task<decimal> ActualizarValorCuota(decimal valor, DateTime FechaActualizacion);
    }
}

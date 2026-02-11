using APIClub.Application.Common;
using APIClub.Application.Dtos.Cuota;
using APIClub.Domain.ModuloGestionCuotas.Models;

namespace APIClub.Domain.GestionSocios.Repositories
{
    public interface ICuotaRepository
    {
        Task<decimal> ObtenerValorCuota();
        Task<decimal> ActualizarValorCuota(decimal valor, DateTime FechaActualizacion);
        void RegistrarCuotas(IEnumerable<Cuota> cuotas);

        Task<PagedResult<CuotaConSocioDto>> ObtenerCuotasPorFechaPago(DateOnly fechaPago, int pageNumber, int pageSize);
        Task<PagedResult<CuotaConSocioDto>> ObtenerCuotasPorPeriodo(int anio, int semestre, int pageNumber, int pageSize);
    }
}

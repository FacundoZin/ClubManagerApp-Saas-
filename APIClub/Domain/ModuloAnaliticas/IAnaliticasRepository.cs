using APIClub.Application.Dtos.Analiticas;

namespace APIClub.Domain.Analiticas
{
    public interface IAnaliticasRepository
    {
        // Socios
        Task<int> GetSociosActivosCount();
        Task<int> GetAltasByPeriodo(int anio, int? mes, int? semestre);
        Task<int> GetBajasByPeriodo(int anio, int? mes, int? semestre);
        Task<int[]> GetAltasByMes(int anio, int? semestre);
        Task<int[]> GetBajasByMes(int anio, int? semestre);
        Task<int[]> GetCantidadSociosByMes(int anio, int? semestre);
        Task<decimal> GetTasaMorosidad(int anio, int semestre);

        // Ingresos
        Task<ResumenIngresosDto> GetResumenIngresos(int anio, int? mes, int? semestre);
        Task<List<IngresoMensualDto>> GetIngresosMesAMes(int anio, int? semestre);

        // Distribución
        Task<DistribucionFormaPagoDto> GetDistribucionFormaPago(int anio, int? semestre);
    }
}

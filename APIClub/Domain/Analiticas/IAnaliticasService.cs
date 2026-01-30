using APIClub.Application.Common;
using APIClub.Application.Dtos.Analiticas;

namespace APIClub.Domain.Analiticas
{
    public interface IAnaliticasService
    {
        Task<Result<ResumenSociosDto>> GetResumenSocios(int anio, int? mes, int? semestre);
        Task<Result<List<MovimientoMensualSociosDto>>> GetMovimientoDeSocios(int anio, int? semestre);
        Task<Result<ResumenIngresosDto>> GetResumenIngresos(int anio, int? mes, int? semestre);
        Task<Result<List<IngresoMensualDto>>> GetHistorialIngresos(int anio, int? semestre);
        Task<Result<DistribucionFormaPagoDto>> GetDistribucionFormaPago(int anio, int? semestre);
    }
}

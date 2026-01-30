using APIClub.Application.Common;
using APIClub.Application.Dtos.Analiticas;
using APIClub.Domain.Analiticas;

namespace APIClub.Application.Services
{
    public class AnaliticasService : IAnaliticasService
    {
        private readonly IAnaliticasRepository _repository;

        public AnaliticasService(IAnaliticasRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ResumenSociosDto>> GetResumenSocios(int anio, int? mes, int? semestre)
        {
            try
            {
                var resumen = new ResumenSociosDto();

                resumen.CantidadSociosActivos = await _repository.GetSociosActivosCount();
                resumen.AltasEnPeriodo = await _repository.GetAltasByPeriodo(anio, mes, semestre);
                resumen.BajasEnPeriodo = await _repository.GetBajasByPeriodo(anio, mes, semestre);

                // Morosidad siempre se calcula sobre el semestre actual del año consultado o el semestre solicitado
                int semestreCalculo = semestre ?? (DateTime.Now.Month <= 6 ? 1 : 2);
                resumen.TasaMorosidad = await _repository.GetTasaMorosidad(anio, semestreCalculo);

                return Result<ResumenSociosDto>.Exito(resumen);
            }
            catch (Exception ex)
            {
                return Result<ResumenSociosDto>.Error($"Error al obtener analíticas de socios: {ex.Message}", 500);
            }
        }

        public async Task<Result<List<MovimientoMensualSociosDto>>> GetMovimientoDeSocios(int anio, int? semestre)
        {
            try
            {
                List<MovimientoMensualSociosDto> movimientoMensualSocios = new List<MovimientoMensualSociosDto>();

                var AltasPorMes = await _repository.GetAltasByMes(anio, semestre);
                var BajasPorMes = await _repository.GetBajasByMes(anio, semestre);
                var CantidadSociosPorMes = await _repository.GetCantidadSociosByMes(anio, semestre);

                // Determinar el rango de meses a procesar
                int mesInicio = semestre.HasValue ? (semestre.Value == 1 ? 1 : 7) : 1;
                int mesFin = semestre.HasValue ? (semestre.Value == 1 ? 6 : 12) : 12;

                // Construir el DTO para cada mes
                for (int mes = mesInicio; mes <= mesFin; mes++)
                {
                    movimientoMensualSocios.Add(new MovimientoMensualSociosDto
                    {
                        Anio = anio,
                        Mes = mes,
                        CantidadAltas = AltasPorMes[mes - 1],
                        CantidadBajas = BajasPorMes[mes - 1],
                        CantidadDeSocios = CantidadSociosPorMes[mes - 1]
                    });
                }

                return Result<List<MovimientoMensualSociosDto>>.Exito(movimientoMensualSocios);
            }
            catch (Exception ex)
            {
                return Result<List<MovimientoMensualSociosDto>>.Error($"Error al obtener analíticas de socios: {ex.Message}", 500);
            }
        }

        public async Task<Result<ResumenIngresosDto>> GetResumenIngresos(int anio, int? mes, int? semestre)
        {
            try
            {
                var resumenIngresos = await _repository.GetResumenIngresos(anio, mes, semestre);

                return Result<ResumenIngresosDto>.Exito(resumenIngresos);
            }
            catch (Exception ex)
            {
                return Result<ResumenIngresosDto>.Error($"Error al obtener analíticas de ingresos: {ex.Message}", 500);
            }
        }

        public async Task<Result<List<IngresoMensualDto>>> GetHistorialIngresos(int anio, int? semestre)
        {
            try
            {
                var historialIngresos = await _repository.GetIngresosMesAMes(anio, semestre);

                return Result<List<IngresoMensualDto>>.Exito(historialIngresos);
            }
            catch (Exception ex)
            {
                return Result<List<IngresoMensualDto>>.Error($"Error al obtener analíticas de ingresos: {ex.Message}", 500);
            }
        }

        public async Task<Result<DistribucionFormaPagoDto>> GetDistribucionFormaPago(int anio, int? semestre)
        {
            try
            {
                var dto = await _repository.GetDistribucionFormaPago(anio, semestre);
                return Result<DistribucionFormaPagoDto>.Exito(dto);
            }
            catch (Exception ex)
            {
                return Result<DistribucionFormaPagoDto>.Error($"Error al obtener distribución de pagos: {ex.Message}", 500);
            }
        }
    }
}

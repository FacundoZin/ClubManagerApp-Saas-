using APIClub.Application.Common;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.Enums;
using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Domain.GestionSocios
{
    public interface ICuotasService
    {
        Task<Result<object>> RegistrarPagosCuotas(int idSocio, List<PeriodoAdeudadoDto> periodos, FormasDePago formaPago);
        Task<Result<object>> RegistrarPagoCuoataOnline(PaymentToken token);
        Task<Result<object>> ActualizarValorCuota(decimal nuevoValor);
        Task<Result<object?>> RegistrarPagosCuotasCobrador(int idSocio, List<PeriodoAdeudadoDto> periodos, int idCobrador);

    }
}

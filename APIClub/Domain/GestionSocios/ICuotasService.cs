using APIClub.Application.Common;
using APIClub.Domain.Enums;
using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Domain.GestionSocios
{
    public interface ICuotasService
    {
        Task<Result<object>> RegistrarPagoCuoata(int idSocio, FormasDePago formaPago);
        Task<Result<object>> RegistrarPagoCuoataOnline(PaymentToken token);
        Task<Result<object>> ActualizarValorCuota(decimal nuevoValor);
       
    }
}

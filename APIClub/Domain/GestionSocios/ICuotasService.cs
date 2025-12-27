using APIClub.Common;
using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Enums;

namespace APIClub.Domain.GestionSocios
{
    public interface ICuotasService
    {
        Task<Result<object>> RegistrarPagoCuoata(int idSocio, FormasDePago formaPago);
        Task<Result<object>> RegistrarPagoCuoataOnline(PaymentToken token);
        Task<Result<object>> ActualizarValorCuota(decimal nuevoValor);
       
    }
}

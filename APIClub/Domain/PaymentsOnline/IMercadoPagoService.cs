using APIClub.Common;
using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Dtos.Payment;

namespace APIClub.Domain.PaymentsOnline
{
    public interface IMercadoPagoService
    {
        Task<string> CreatePaymentPreference(string title, decimal montoCuota,string externalReference);
        Task<mpPaymentInfoDto> GetPayment(string paymentId);
        Task<Result<ProcessPaymentResponseDto>> ProcessPayment(ProcessPaymentRequestDto request, decimal amount, string description);
    }
}

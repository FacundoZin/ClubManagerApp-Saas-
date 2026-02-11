using APIClub.Application.Common;
using APIClub.Application.Dtos.Payment;
using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Domain.PaymentsOnline.ExternalServices
{
    public interface IMercadoPagoService
    {
        Task<string> CreatePaymentPreference(string title, decimal montoCuota,string externalReference);
        Task<mpPaymentInfoDto> GetPayment(string paymentId);
        Task<Result<ProcessPaymentResponseDto>> ProcessPayment(ProcessPaymentRequestDto request, decimal amount, string description);
    }
}

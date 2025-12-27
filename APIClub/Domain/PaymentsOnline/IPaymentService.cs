using APIClub.Common;
using APIClub.Dtos.Payment;
using System.Text.Json;

namespace APIClub.Domain.PaymentsOnline
{
    public interface IPaymentService
    {
        Task<Result<PortalPaymentView>> InitPaymentProcess(Guid tokenId);
        Task<Result<ProcessPaymentResponseDto>> ProcessPayment(ProcessPaymentRequestDto request);
        Task RegistrarPago(JsonElement notification);
    }
}

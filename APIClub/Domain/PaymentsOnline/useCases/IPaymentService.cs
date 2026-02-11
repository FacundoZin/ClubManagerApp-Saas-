using APIClub.Application.Common;
using APIClub.Application.Dtos.Payment;

namespace APIClub.Domain.PaymentsOnline.useCases
{
    public interface IPaymentService
    {
        Task<Result<PortalPaymentViewDto>> InitPaymentProcess(Guid tokenId);
        Task<Result<object?>> ProcessPayment(ProcessPaymentRequestDto request);
        Task RegistrarPago(MercadoPagoWebhookDto notification);
        Task<Result<InfoComprobanteDto?>> getComprobante(Guid tokenId);
    }
}

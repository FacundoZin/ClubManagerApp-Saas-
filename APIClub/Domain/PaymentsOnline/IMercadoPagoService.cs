using APIClub.Common;
using APIClub.Dtos.Payment;

namespace APIClub.Domain.PaymentsOnline
{
    public interface IMercadoPagoService
    {
        Task<string> CreatePaymentPreference(string title, decimal montoCuota);
        Task<mpPaymentInfo> GetPayment(string paymentId);
    }
}

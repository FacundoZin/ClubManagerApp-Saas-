using APIClub.Dtos.Payment;

namespace APIClub.Domain.PaymentsOnline
{
    public interface IMercadoPagoService
    {
        Task<string?> CreatePayment(string External_Reference, decimal monto, string descripcion);
        Task<mpPaymentInfo> GetPayment(string paymentId);
    }
}

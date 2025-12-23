using APIClub.Domain.PaymentsOnline;
using APIClub.Dtos.Payment;

namespace APIClub.Services
{
    public class MPService : IMercadoPagoService
    {
        public Task<string?> CreatePayment(string External_Reference, decimal monto, string descripcion)
        {
            throw new NotImplementedException();
        }

        public Task<mpPaymentInfo> GetPayment(string paymentId)
        {
            throw new NotImplementedException();
        }
    }
}

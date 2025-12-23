using APIClub.Common;
using APIClub.Domain.PaymentsOnline;
using APIClub.Dtos.Payment;

namespace APIClub.Services
{
    public class PaymentService : IPaymentService
    {
        public Task<Result<PortalPaymentView>> InitProcessPayment()
        {
            throw new NotImplementedException();
        }
    }
}

using APIClub.Domain.PaymentsOnline;
using Quartz;

namespace APIClub.Infrastructure.JobsProgramados
{
    public class CreatePaymentTokensJob : IJob
    {
        private readonly IPaymentTokenService _paymentTokenService;

        public CreatePaymentTokensJob(IPaymentTokenService paymentTokenService)
        {
            _paymentTokenService = paymentTokenService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}

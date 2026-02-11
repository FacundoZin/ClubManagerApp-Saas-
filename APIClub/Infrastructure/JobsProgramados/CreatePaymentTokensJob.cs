using APIClub.Domain.PaymentsOnline.useCases;
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

        public async Task Execute(IJobExecutionContext context)
        {
            await _paymentTokenService.CreatePaymentTokens();
        }
    }
}

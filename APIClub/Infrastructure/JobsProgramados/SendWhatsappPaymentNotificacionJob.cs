using APIClub.Domain.Notificaciones.Services;
using Quartz;

namespace APIClub.Infrastructure.JobsProgramados
{
    public class SendWhatsappPaymentNotificacionJob : IJob
    {
        private readonly INotificationsService _notificationsService;

        public SendWhatsappPaymentNotificacionJob (INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _notificationsService.NotificarPagodeCuota();
        }
    }
}

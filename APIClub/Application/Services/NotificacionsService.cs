using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.Notificaciones;
using APIClub.Domain.Notificaciones.Infra;
using APIClub.Domain.Notificaciones.Services;

namespace APIClub.Application.Services
{
    public class NotificacionsService : INotificationsService
    {
        private readonly IWhatsappService _whatsappService;
        private readonly ISocioRepository _socioRepository;

        public Task NotificarPagoCuotaTestJob()
        {
            throw new NotImplementedException();
        }

        public Task NotificarPagoCuotaTestWsp()
        {
            throw new NotImplementedException();
        }

        public Task NotificarPagodeCuota()
        {
            // _whatsappService.SendWhatsAppPaymentMessage();
        }
    }
}

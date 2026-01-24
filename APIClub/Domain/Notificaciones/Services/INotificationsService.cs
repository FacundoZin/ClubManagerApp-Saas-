namespace APIClub.Domain.Notificaciones.Services
{
    public interface INotificationsService
    {
        Task NotificarPagoCuotaTestWsp();
        Task NotificarPagodeCuota();

        // Task NotificarEvento()
        // Task Notificarloquesea()
    }
}

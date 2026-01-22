namespace APIClub.Domain.Notificaciones.Services
{
    public interface INotificationsService
    {
        Task NotificarPagoCuotaTestJob();
        Task NotificarPagoCuotaTestWsp();
        Task NotificarPagodeCuota();

        // Task NotificarEvento()
        // Task Notificarloquesea()
    }
}

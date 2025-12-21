using APIClub.Common;

namespace APIClub.Domain.Background
{
    public interface INotifyService
    {
        Task<Result<object?>> SendNotificacionPagoCuota();
        Task<Result<object?>> SendWhatsAppTest(string telefono, string nombre);
    }
}

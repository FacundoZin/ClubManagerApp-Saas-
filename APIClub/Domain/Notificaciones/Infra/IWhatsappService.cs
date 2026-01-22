using APIClub.Application.Common;

namespace APIClub.Domain.Notificaciones.Infra
{
    public interface IWhatsappService
    {
        Task<Result<object?>> SendNotificacionPagoCuota();
        Task<HttpResponseMessage> SendWhatsAppPaymentMessage(string telefono, string nombreSocio, string semestreTexto, string anio);
        Task<Result<object?>> SendWhatsAppTest(string telefono, string nombre);
    }
}

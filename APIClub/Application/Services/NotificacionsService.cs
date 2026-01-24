using APIClub.Application.Helpers;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.Notificaciones.Infra;
using APIClub.Domain.Notificaciones.Services;

namespace APIClub.Application.Services
{
    public class NotificacionsService : INotificationsService
    {
        private readonly IWhatsappService _whatsappService;
        private readonly ISocioRepository _socioRepository;

        public NotificacionsService(IWhatsappService whatsappService, ISocioRepository socioRepository)
        {
            _whatsappService = whatsappService;
            _socioRepository = socioRepository;
        }

        public Task NotificarPagoCuotaTestWsp()
        {
            throw new NotImplementedException();
        }

        public async Task NotificarPagodeCuota()
        {
            var fechaActual = DateTime.Now;
            int anio = fechaActual.Year;
            int semestre = fechaActual.Month <= 6 ? 1 : 2;
            string semestreTexto = semestre.GetSemestreText();

            int pageNumber = 1;
            int pageSize = 20; // Tamaño de  para no saturar
            bool hasMore = true;

            while (hasMore)
            {
                var deudores = await _socioRepository.GetSociosDeudoresWithPreferenceLinkDePagoPaginado(anio, semestre, pageNumber, pageSize);

                if (deudores.Count == 0)
                {
                    hasMore = false;
                    break;
                }

                foreach (var socio in deudores)
                {
                    if (!string.IsNullOrEmpty(socio.Telefono))
                    {
                        await _whatsappService.SendWhatsAppPaymentMessage(
                            socio.Telefono, 
                            $"{socio.Nombre} {socio.Apellido}", 
                            semestreTexto, 
                            anio.ToString()
                        );
                    }
                }

                if (deudores.Count < pageSize)
                {
                    hasMore = false;
                }
                else
                {
                    //nota: si alguien realiza el pago de una cuota mentras este prceso esta en ejecucion 
                    // se va a saltar la notificaccion de un socio ya que la lista de socios deudores se va 
                    // a acortar y la paginacion va pasar por alto algunos socios.
                    // por ahora esto puede quedar asi ya que es un proceso que se ejecuta una vez por mes y en un horario donde nadie paga.
                    // en el futuro si se hace un Saas con esto se deberia modificar el esquema para marcar a los socios como notificados 
                    // e ir haciendo la paginacion solicitiando simepre la pagina 1, la cual va a ir recorreindo todos los socios porque al marcarse un 
                    // socio de la lista se excluye de la busqueda.
                    pageNumber++;
                }
            }
        }
    }
}

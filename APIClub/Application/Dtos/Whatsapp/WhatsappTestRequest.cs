using APIClub.Domain.GestionSocios.Validations;

namespace APIClub.Application.Dtos.Whatsapp
{
    public class WhatsappTestRequest
    {
        public string Telefono { get; set; }
        public string nombreSocio { get; set; }
    }
}

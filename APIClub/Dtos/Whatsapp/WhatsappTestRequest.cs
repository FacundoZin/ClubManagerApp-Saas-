using APIClub.Domain.GestionSocios.Validations;

namespace APIClub.Dtos.Whatsapp
{
    public class WhatsappTestRequest
    {
        [PhoneNumer]
        public string Telefono { get; set; }
        public string nombreSocio { get; set; }
    }
}

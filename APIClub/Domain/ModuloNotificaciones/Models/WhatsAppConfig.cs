namespace APIClub.Domain.Notificaciones.Models
{
    public class WhatsAppConfig
    {
        public string AccessToken { get; set; } 
        public string PhoneNumberId { get; set; } 
        public string ApiVersion { get; set; } 
        public string VerifyToken { get; set; }
    }
}

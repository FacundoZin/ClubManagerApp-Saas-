namespace APIClub.Domain.Notificaciones
{
    public class WhatsAppConfig
    {
        public string AccessToken { get; set; } = string.Empty;
        public string PhoneNumberId { get; set; } = string.Empty;
        public string ApiVersion { get; set; } = "v18.0";
    }
}

using System.Text.Json.Serialization;

namespace APIClub.Domain.Notificaciones.Models
{
    public class WhatsAppMessageRequest
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; } = "whatsapp";

        [JsonPropertyName("to")]
        public string To { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "template";

        [JsonPropertyName("template")]
        public WhatsAppTemplate Template { get; set; } = new();
    }

    public class WhatsAppTemplate
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public WhatsAppLanguage Language { get; set; } = new();

        [JsonPropertyName("components")]
        public List<WhatsAppComponent> Components { get; set; } = new();
    }

    public class WhatsAppLanguage
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = "es";
    }

    public class WhatsAppComponent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("parameters")]
        public List<WhatsAppParameter> Parameters { get; set; } = new();
    }

    public class WhatsAppParameter
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    // Para mensajes de texto simples (no template)
    public class WhatsAppTextMessageRequest
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; } = "whatsapp";

        [JsonPropertyName("recipient_type")]
        public string RecipientType { get; set; } = "individual";

        [JsonPropertyName("to")]
        public string To { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        [JsonPropertyName("text")]
        public WhatsAppText Text { get; set; } = new();
    }

    public class WhatsAppText
    {
        [JsonPropertyName("preview_url")]
        public bool PreviewUrl { get; set; } = false;

        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;
    }
}

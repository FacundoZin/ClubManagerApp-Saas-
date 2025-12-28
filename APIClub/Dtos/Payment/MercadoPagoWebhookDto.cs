using System.Text.Json.Serialization;
using System.Text.Json;

public class MercadoPagoWebhookDto
{
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("api_version")]
    public string? ApiVersion { get; set; }

    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }

    [JsonPropertyName("date_created")]
    public DateTime? DateCreated { get; set; }

    [JsonPropertyName("data")]
    public WebhookData? Data { get; set; }

    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("live_mode")]
    public bool? LiveMode { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }
}

public class WebhookData
{
    [JsonPropertyName("id")]
    public JsonElement RawId { get; set; }

    [JsonIgnore]
    public string? Id =>
        RawId.ValueKind switch
        {
            JsonValueKind.Number => RawId.GetInt64().ToString(),
            JsonValueKind.String => RawId.GetString(),
            _ => null
        };
}

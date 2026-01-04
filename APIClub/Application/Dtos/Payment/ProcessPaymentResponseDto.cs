namespace APIClub.Application.Dtos.Payment
{
    /// <summary>
    /// DTO para responder al frontend con el resultado del procesamiento de pago
    /// </summary>
    public class ProcessPaymentResponseDto
    {
        public long? paymentId { get; set; }
        public string status { get; set; }
        public string status_detail { get; set; }
    }
}

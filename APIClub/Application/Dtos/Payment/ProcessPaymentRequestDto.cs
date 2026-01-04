namespace APIClub.Application.Dtos.Payment
{
    /// <summary>
    /// DTO para recibir datos del Payment Brick desde el frontend
    /// </summary>
    public class ProcessPaymentRequestDto
    {
        public string token { get; set; }
        public string issuer_id { get; set; }
        public string payment_method_id { get; set; }
        public decimal transaction_amount { get; set; }
        public int installments { get; set; }
        public PayerDto payer { get; set; }
        public Guid externalReference { get; set; }
    }

    public class PayerDto
    {
        public string email { get; set; }
        public IdentificationDto identification { get; set; }
    }

    public class IdentificationDto
    {
        public string type { get; set; }
        public string number { get; set; }
    }
}

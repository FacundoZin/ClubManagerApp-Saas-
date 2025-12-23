namespace APIClub.Dtos.Payment
{
    public class mpPaymentInfo
    {
        public string PaymentId { get; set; } = null!;
        public string Status { get; set; } = null!;          // approved, rejected, pending
        public string ExternalReference { get; set; } = null!;
        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; }
        public DateTime? ApprovedAt { get; set; }

    }
}

namespace APIClub.Domain.PaymentsOnline.Modelos
{
    public class PaymentToken
    {
        public Guid Id { get; set; }
        public string nombreSocio { get; set; }
        public int IdSocio { get; set; }
        public int anio { get; set; }
        public int semestre { get; set; }
        public DateOnly FechaExpiracion { get; set; }
        public decimal monto { get; set; }
        public bool usado { get; set; } = false;
        public string? Preference_Id { get; set; }
    }
}

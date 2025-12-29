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

        // ni bien se obtiene el estado approved del pago se hace una transaccion atomica diretcamente en la db que marca el token como
        // usado evitando condicion de carrera y por consecuencia pagos duplicados
        public bool usado { get; set; } = false;
        public string? Preference_Id { get; set; }

        public string? PaymentStatus { get; set; }
        public string? StatusDetail { get; set; }
    }
}

namespace APIClub.Application.Dtos.Payment
{
    public class PortalPaymentViewDto
    {
        public string nombreSocio { get; set; }
        public string anioPago { get; set; }
        public string semestrePago { get; set; }
        public decimal monto { get; set; }
        public Guid externalReference { get; set; }
        public string Preference_Id { get; set; }
        public bool AlreadyPaid { get; set; } = false;
        public InfoComprobanteDto? Comprobante { get; set; }
    }
}

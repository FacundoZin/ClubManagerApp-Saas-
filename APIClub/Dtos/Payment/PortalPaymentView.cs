namespace APIClub.Dtos.Payment
{
    public class PortalPaymentView
    {
        public string nombreSocio { get; set; }
        public string anioPago { get; set; }
        public string semestrePago { get; set; }
        public decimal monto { get; set; }
        public Guid externalReference { get; set; }
        public string Preference_Id { get; set; }
    }
}

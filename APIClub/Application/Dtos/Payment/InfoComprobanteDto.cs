using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Application.Dtos.Payment
{
    public class InfoComprobanteDto
    {
        public string nombreSocio { get; set; } = string.Empty;
        public string dniSocio { get; set; } = string.Empty;
        public string anioPago { get; set; } = string.Empty;
        public string semestrePagoText { get; set; } = string.Empty;
        public string monto { get; set; } = string.Empty;
    }
}

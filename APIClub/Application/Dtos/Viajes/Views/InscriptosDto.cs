namespace APIClub.Application.Dtos.Viajes.Views
{
    public class InscriptosDto
    {
        public int Id { get; set; }
        public string NombreSocio { get; set; } = string.Empty;
        public string DniSocio { get; set; } = string.Empty;
        public string TelefonoSocio { get; set; } = string.Empty;
        public decimal MontoAbonado { get; set; }
        public decimal MontoPendiente { get; set; }
        public bool Cancelado { get; set; }
    }
}

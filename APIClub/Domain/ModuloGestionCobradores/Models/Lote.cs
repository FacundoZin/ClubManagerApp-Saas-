namespace APIClub.Domain.ModuloGestionCobradores.Models
{
    public class Lote
    {
        public int Id { get; set; }
        public string NombreLote { get; set; } = string.Empty;
        public string CalleNorte { get; set; } = string.Empty;
        public string CalleSur { get; set; } = string.Empty;
        public string CalleEste { get; set; } = string.Empty;
        public string CalleOeste { get; set; } = string.Empty;
    }
}

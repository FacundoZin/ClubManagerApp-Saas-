namespace APIClub.Application.Dtos.Lote
{
    public class PreviewLote
    {
        public int Id { get; set; }
        public string NombreLote { get; set; }
        public string CalleNorte { get; set; } = string.Empty;
        public string CalleSur { get; set; } = string.Empty;
        public string CalleEste { get; set; } = string.Empty;
        public string CalleOeste { get; set; } = string.Empty;
    }
}

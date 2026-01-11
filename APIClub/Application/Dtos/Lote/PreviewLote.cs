namespace APIClub.Application.Dtos.Lote
{
    public class PreviewLote
    {
        public int Id { get; set; }
        public string NombreLote { get; set; }
        public string Calle1 { get; set; } = string.Empty;
        public string Calle2 { get; set; } = string.Empty;
        public string Calle3 { get; set; } = string.Empty;
        public string Calle4 { get; set; } = string.Empty;
    }
}

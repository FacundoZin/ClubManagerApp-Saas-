namespace APIClub.Application.Dtos.Viajes.Views
{
    public class FullViewViajeDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int Dias { get; set; }
        public int Noches { get; set; }
        public DateOnly Fechasalida { get; set; }
        public int? VentasParaLiberado { get; set; }
        public decimal ValorBase { get; set; }
        public List<VarianteViajeWithInscriptosDto> Variantes { get; set; } = new List<VarianteViajeWithInscriptosDto>();
        public decimal TotalRecaudado { get; set; }
        public decimal MontoComision { get; set; }
        public decimal MontoParaAgencia { get; set; }
        public decimal TotalPendiente { get; set; }
        public int TotalInscriptos { get; set; }
        public int TotalCancelados { get; set; }
        public int TotalLiberados { get; set; }
    }
}

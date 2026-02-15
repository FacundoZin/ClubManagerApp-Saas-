namespace APIClub.Application.Dtos.Viajes.Views
{
    public class PreviewViajeDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int Dias { get; set; }
        public int Noches { get; set; }
        public DateOnly Fechasalida { get; set; }
        public int? VentasParaLiberado { get; set; }
        public decimal ValorBase { get; set; }
        public decimal PorcentajeComision { get; set; }
    }
}

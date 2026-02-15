using APIClub.Domain.Enums;

namespace APIClub.Domain.ModuloGestionViajes.Models
{
    public class Viaje
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int Dias { get; set; }
        public int Noches { get; set; }
        public DateOnly Fechasalida { get; set; }
        public decimal PorcentajeComision { get; set; }
        public int? VentasParaLiberado { get; set; }
        public decimal ValorBase { get; set; }

        public List<VarianteViaje> Variantes { get; set; } = new List<VarianteViaje>();
    }
}

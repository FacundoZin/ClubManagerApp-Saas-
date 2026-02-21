using APIClub.Domain.Enums;

namespace APIClub.Domain.ModuloGestionViajes.Models
{
    public class VarianteViaje
    {
        public int Id { get; set; }

        public string NombreVariante { get; set; } = string.Empty;
        public decimal ValorViaje { get; set; }
        public decimal ValorSeña { get; set; }

        //relaciones
        public int IdViaje { get; set; }
        public Viaje Viaje { get; set; } = null!;

        public List<InscriptoViaje> Inscriptos { get; set; } = new List<InscriptoViaje>();
        public RegimenViaje? Regimen { get; set; }
        public string? TipoDeButaca { get; set; } = string.Empty;
    }
}

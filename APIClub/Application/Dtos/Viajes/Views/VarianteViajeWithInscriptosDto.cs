using APIClub.Domain.Enums;

namespace APIClub.Application.Dtos.Viajes.Views
{
    public class VarianteViajeWithInscriptosDto
    {
        public int Id { get; set; }

        public string NombreVariante { get; set; } = string.Empty;
        public decimal ValorViaje { get; set; }
        public decimal ValorSeña { get; set; }
        public RegimenViaje? Regimen { get; set; }
        public string? TipoDeButaca { get; set; } 
        public List<InscriptosDto> Inscriptos { get; set; } = new List<InscriptosDto>();
    }
}

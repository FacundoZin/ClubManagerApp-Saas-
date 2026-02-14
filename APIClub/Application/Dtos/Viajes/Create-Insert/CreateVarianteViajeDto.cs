using APIClub.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Create
{
    public class CreateVarianteViajeDto
    {
        public int IdViaje { get; set; }

        [Required(ErrorMessage = "El nombre de la variante es obligatorio")]
        public string NombreVariante { get; set; } = string.Empty;

        [Required(ErrorMessage = "El valor del viaje es obligatorio")]
        public decimal ValorViaje { get; set; }

        [Required(ErrorMessage = "El valor de la seña es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "El valor de la seña debe ser mayor a 0")]
        public decimal ValorSeña { get; set; }

        [Required(ErrorMessage = "El régimen del viaje es obligatorio")]
        public RegimenViaje Regimen { get; set; }

        [Required(ErrorMessage = "El tipo de butaca es obligatorio")]
        public string TipoDeButaca { get; set; } = string.Empty;
    }
}

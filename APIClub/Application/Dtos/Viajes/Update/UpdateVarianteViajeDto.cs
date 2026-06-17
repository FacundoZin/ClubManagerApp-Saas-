using APIClub.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Update
{
    public class UpdateVarianteViajeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el nombre de la variante")]
        public string NombreVariante { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor ingrese el valor del viaje")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El valor del viaje debe ser mayor a 0")]
        public decimal ValorViaje { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el valor de la seña")]
        [Range(0, double.MaxValue, ErrorMessage = "El valor de la seña no puede ser negativo")]
        public decimal ValorSeña { get; set; }

        public RegimenViaje? Regimen { get; set; }

        public string? TipoDeButaca { get; set; } = string.Empty;
    }
}

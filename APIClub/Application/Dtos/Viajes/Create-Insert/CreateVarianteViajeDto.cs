using APIClub.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Create
{
    public class CreateVarianteViajeDto
    {
        public int IdViaje { get; set; }

        [Required(ErrorMessage = "Porfavor ingrese el nombre de la variante de viaje")]
        public string NombreVariante { get; set; } = string.Empty;

        [Required(ErrorMessage = "Porfavor ingrese el valor del viaje")]
        public decimal ValorViaje { get; set; }

        [Required(ErrorMessage = "Porfavor seleccione un valor de seña")]
        [Range(1, double.MaxValue, ErrorMessage = "El valor de la seña debe ser mayor a 0")]
        public decimal ValorSeña { get; set; }

        public RegimenViaje? Regimen { get; set; }

        public string? TipoDeButaca { get; set; } = string.Empty;
    }
}

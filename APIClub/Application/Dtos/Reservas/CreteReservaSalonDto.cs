using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Reservas
{
    public class CreteReservaSalonDto : IValidatableObject
    {
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateOnly Fecha { get; set; }

        [Required(ErrorMessage = "El salón es requerido")]
        public int SalonId { get; set; }

        [Required(ErrorMessage = "El DNI del socio es requerido")]
        public string DniSocio { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "El importe no puede ser negativo")]
        public decimal Importe { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El monto pagado no puede ser negativo")]
        public decimal TotalPagado { get; set; } = 0;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TotalPagado > Importe)
            {
                yield return new ValidationResult(
                    "el monto abonado no puede ser mayor al importe total de la reserva",
                    new[] { nameof(Importe), nameof(TotalPagado) }
                );
            }
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace APIClub.Dtos.Reservas
{
    public class CreteReservaSalonDto
    {
        public string Titulo { get; set; }

        [Required]
        public DateOnly Fecha { get; set; }

        [Required]
        public int SalonId { get; set; }

        [Required]
        public string DniSocio { get; set; } = string.Empty;

        public decimal Importe { get; set; }
        public decimal TotalPagado { get; set; }
    }
}

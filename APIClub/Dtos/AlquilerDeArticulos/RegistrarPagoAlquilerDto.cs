using System.ComponentModel.DataAnnotations;

namespace APIClub.Dtos.AlquilerArticulos
{
    public class RegistrarPagoAlquilerDto
    {
        [Required]
        public int AlquilerId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public int Monto { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.AlquilerDeArticulos
{
    public class RegistrarPagoAlquilerDto
    {
        [Required]
        public int AlquilerId { get; set; }

        [Required(ErrorMessage = "Porfavor ingrese un MONTO")]
        [Range(1, int.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public int Monto { get; set; }
    }
}

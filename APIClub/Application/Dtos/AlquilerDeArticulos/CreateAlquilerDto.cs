using APIClub.Application.Dtos.ItemsAlquiler;
using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.AlquilerDeArticulos
{
    public class CreateAlquilerDto
    {
        [Required(ErrorMessage = "El DNI del socio es obligatorio")]
        public int idSocio { get; set; }

        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos un artículo")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un artículo")]
        public List<AddItemToAlquilerDto> Items { get; set; } = new List<AddItemToAlquilerDto>();
    }
}

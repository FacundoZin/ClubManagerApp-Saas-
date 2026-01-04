using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.ItemsAlquiler
{
    public class AddItemToAlquilerDto
    {
        [Required(ErrorMessage = "El artículo es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "el id no puede ser 0")]
        public int ArticuloId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "debe alquilarse almenos un articulo por item, de lo contrario elimine el item del alquiler")]
        public int Cantidad { get; set; }
    }
}

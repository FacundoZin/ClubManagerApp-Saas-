using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.ItemsAlquiler
{
    public class ModifyItemQuantityDto
    {
        [Required]
        public int ItemId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "debe alquilarse almenos un articulo por item, de lo contrario elimine el item del alquiler")]
        public int NuevaCantidad { get; set; }
    }
}

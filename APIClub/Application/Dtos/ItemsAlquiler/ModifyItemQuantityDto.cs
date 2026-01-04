using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.ItemsAlquiler
{
    public class ModifyItemQuantityDto
    {
        [Required]
        public int ItemId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int NuevaCantidad { get; set; }
    }
}

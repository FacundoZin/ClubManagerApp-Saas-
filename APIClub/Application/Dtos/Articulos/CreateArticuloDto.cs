using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Articulos
{
    public class CreateArticuloDto
    {
        [Required(ErrorMessage = "El nombre del artículo es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Porfavor ingrese un PRECIO de alquiler al articulo")]
        [Range(1, int.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public int PrecioAlquiler { get; set; }
    }
}

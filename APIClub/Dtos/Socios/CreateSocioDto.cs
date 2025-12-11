using System.ComponentModel.DataAnnotations;

namespace APIClub.Dtos.Socios
{
    public class CreateSocioDto
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string Dni { get; set; }

        public string? Telefono { get; set; }
        public string? Direcccion { get; set; }
        public string? Lote { get; set; }
        public string? Localidad { get; set; }
    }
}

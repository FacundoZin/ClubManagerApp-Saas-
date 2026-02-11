using System.ComponentModel.DataAnnotations;
using APIClub.Domain.Enums;

namespace APIClub.Application.Dtos.Socios
{
    public class UpdateSocio
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string Dni { get; set; }

        public string? Telefono { get; set; }
        public string? Direcccion { get; set; }
        public int? IdLote { get; set; }
        public string? Localidad { get; set; }

        [Required(ErrorMessage = "La forma de pago es requerida")]
        public MetodosDePago PreferenciaDePago { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using APIClub.Domain.Enums;

namespace APIClub.Application.Dtos.Socios
{
    public class CreateSocioDto
    {
        [Required(ErrorMessage = "porfavor ingrese el NOMBRE del socio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "porfavor ingrese el APELLIDO del socio")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "porfavor ingrese el DNI del socio")]
        public string Dni { get; set; }

        public string? Telefono { get; set; }
        public string? Direcccion { get; set; }
        public int? IdLote { get; set; }
        public string? Localidad { get; set; }

        [Required(ErrorMessage = "Porfavor seleccione la preferencia de pago del socio")]
        public MetodosDePago PreferenciaDePago { get; set; }
    }
}

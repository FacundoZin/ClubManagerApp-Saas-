using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Create
{
    public class InsertInscriptoViajeDto
    {
        [Required(ErrorMessage = "El número de file es obligatorio")]
        public string NumeroFile { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar al menos un inscripto")]
        public List<InscriptoItemDto> Inscriptos { get; set; } = new();
    }

    public class InscriptoItemDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar una variante")]
        public int VarianteViajeId { get; set; }

        [Required(ErrorMessage = "El monto abonado es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal MontoAbonado { get; set; }

        [Required(ErrorMessage = "El número de recibo es obligatorio")]
        public string NumeroRecibo { get; set; } = string.Empty;
    }
}

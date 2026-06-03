using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Update
{
    public class UpdatePagoViajeDto
    {
        public int IdInscripto { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el monto abonado")]
        public decimal MontoAbonado { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el número de recibo")]
        public string NumeroRecibo { get; set; } = string.Empty;
    }
}
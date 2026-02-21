using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Update
{
    public class UpdatePagoViajeDto
    {
        public int IdInscripto { get; set; }
        [Required(ErrorMessage = "porfavor ingrese el monto abonado")]
        public decimal MontoAbonado { get; set; }
    }
}
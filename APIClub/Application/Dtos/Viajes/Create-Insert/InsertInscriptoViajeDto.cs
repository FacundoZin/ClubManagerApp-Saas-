using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Create
{
    public class InsertInscriptoViajeDto
    {
        public int ViajeVarianteId { get; set; }
        public int SocioId { get; set; }

        [Required(ErrorMessage = "El monto abonado es obligatorio")]
        public decimal MontoAbonado { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Update
{
    public class EditPagoViajeDto
    {
        public int IdInscripto { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el nuevo importe")]
        [Range(0, double.MaxValue, ErrorMessage = "El nuevo importe debe ser mayor o igual a 0")]
        public decimal NuevoMontoAbonado { get; set; }

        public string MotivoModificacion { get; set; } = string.Empty;
    }
}

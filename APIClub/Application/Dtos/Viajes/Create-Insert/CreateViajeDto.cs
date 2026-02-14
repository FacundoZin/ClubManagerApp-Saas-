using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Create
{
    public class CreateViajeDto
    {
        [Required(ErrorMessage = "El título del viaje es obligatorio")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cantidad de días es obligatoria")]
        [Range(1, 365, ErrorMessage = "La cantidad de días debe ser mayor a 1 y debe tener un numero valido")]
        public int Dias { get; set; }

        [Required(ErrorMessage = "La cantidad de noches es obligatoria")]
        [Range(1, 365, ErrorMessage = "La cantidad de noches debe ser mayor a 1 y ser un numero valido")]
        public int Noches { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        public DateOnly FechaSalida { get; set; }

        public int? VentasParaLiberado { get; set; }

        [Required(ErrorMessage = "El valor base es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El valor base debe ser mayor a 0")]
        public decimal ValorBase { get; set; }
    }
}

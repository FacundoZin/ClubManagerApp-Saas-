using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Update
{
    public class UpdateViajeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el titulo o nombre del viaje")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor seleccione la cantidad de DIAS del viaje")]
        [Range(1, 365, ErrorMessage = "La cantidad de días debe ser mayor a 1")]
        public int Dias { get; set; }

        [Required(ErrorMessage = "Por favor seleccione la cantidad de NOCHES del viaje")]
        [Range(0, 365, ErrorMessage = "La cantidad de noches debe ser 0 o más")]
        public int Noches { get; set; }

        [Required(ErrorMessage = "Por favor ingrese la fecha de salida del viaje")]
        public DateOnly FechaSalida { get; set; }

        public int? VentasParaLiberado { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el valor base del viaje")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El valor base debe ser mayor a 0")]
        public decimal ValorBase { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el porcentaje de comision")]
        [Range(0, 100, ErrorMessage = "El porcentaje de comision debe estar entre 0 y 100")]
        public decimal PorcentajeComision { get; set; }
    }
}

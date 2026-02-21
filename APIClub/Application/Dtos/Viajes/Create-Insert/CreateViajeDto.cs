using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Viajes.Create
{
    public class CreateViajeDto
    {
        [Required(ErrorMessage = "Porfavor ingrese el titulo o nombre del viaje")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Porfavor seleccione la cantidad de DIAS del viaje")]
        [Range(1, 365, ErrorMessage = "La cantidad de días debe ser mayor a 1 y debe tener un numero valido")]
        public int Dias { get; set; }

        [Required(ErrorMessage = "Porfavor seleccione la cantidad de DIAS del viaje")]
        [Range(1, 365, ErrorMessage = "La cantidad de noches debe ser mayor a 1 y ser un numero valido")]
        public int Noches { get; set; }

        [Required(ErrorMessage = "Porfavor ingrese la fecha de salida del viaje")]
        public DateOnly FechaSalida { get; set; }

        public int? VentasParaLiberado { get; set; }

        [Required(ErrorMessage = "Porfavor ingrese el valor base del viaje")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El valor base debe ser mayor a 0")]
        public decimal ValorBase { get; set; }

        [Required(ErrorMessage = "Porfavor ingrese el porcentaje de comision para su asociacion")]
        [Range(0.1, double.MaxValue, ErrorMessage = "El porcentaje de comision debe ser mayor a 0")]
        public decimal PorcentajeComision { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Lote
{
    public class CreateLoteDto
    {
        [Required(ErrorMessage = "porfavor ingrese el nombre del lote que quiere registrar")]
        public string NombreLote { get; set; } = string.Empty;

        [Required(ErrorMessage = "porfavor ingrese el nombre de la delimitacion NORTE")]
        public string CalleNorte { get; set; } = string.Empty;

        [Required(ErrorMessage = "porfavor ingrese el nombre de la delimitacion SUR")]
        public string CalleSur { get; set; } = string.Empty;

        [Required(ErrorMessage = "porfavor ingrese el nombre de la delimitacion OESTE")]
        public string CalleOeste { get; set; } = string.Empty;

        [Required(ErrorMessage = "porfavor ingrese el nombre de la delimitacion ESTE")]
        public string CalleEste { get; set; } = string.Empty;
    }
}

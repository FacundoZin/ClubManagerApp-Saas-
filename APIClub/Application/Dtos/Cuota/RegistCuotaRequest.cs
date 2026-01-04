using APIClub.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Cuota
{
    public class RegistCuotaRequest
    {
        [Required]
        public int IdSocio { get; set; }
        [Required]
        public FormasDePago FormaPago { get; set; }
    }
    

}

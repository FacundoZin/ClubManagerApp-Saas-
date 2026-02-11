using APIClub.Application.Dtos.Socios;
using APIClub.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Cuota
{
    public class RegistCuotaInRequestDto
    {
        [Required]
        public int IdSocio { get; set; }
        [Required]
        public MetodosDePago FormaPago { get; set; }
        public List<PeriodoAdeudadoDto> Periodos { get; set; } = new List<PeriodoAdeudadoDto>();
    }
}

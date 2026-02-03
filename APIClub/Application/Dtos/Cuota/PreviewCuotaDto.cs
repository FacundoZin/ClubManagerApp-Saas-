using APIClub.Application.Dtos.Socios;

namespace APIClub.Application.Dtos.Cuota
{
    public class PreviewCuotaDto
    {
        public DateOnly? FechaDePago { get; set; }
        public decimal? ImportePagado { get; set; }
        public string? MetodoDePago { get; set; }
        public List<PeriodoCuotasDto> PeriodoAdeudadoDto { get; set; } = new List<PeriodoCuotasDto>();
    }
}

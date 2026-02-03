using System.Collections.Generic;

namespace APIClub.Application.Dtos.Socios
{
    public class SocioDebtPreviewDto : PreviewSocioDto
    {
        public List<PeriodoAdeudadoDto> PeriodosAdeudados { get; set; } = new List<PeriodoAdeudadoDto>();
    }
}

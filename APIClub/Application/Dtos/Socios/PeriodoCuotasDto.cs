using APIClub.Domain.Enums;

namespace APIClub.Application.Dtos.Socios
{
    public class PeriodoCuotasDto 
    {
        public DateOnly? FechaDePago { get; set; }
        public MetodosDePago? MetodoDePago { get; set; }
        public decimal? ImportePagado { get; set; }
        public int anio {  get; set; }
        public int semestre { get; set; }
        public bool pagado { get; set; }
    }
}

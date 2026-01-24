namespace APIClub.Application.Dtos.Cobrador
{
    public class HistorialCobradorDto
    {
        public List<CobroDto> CobrosRealizados { get; set; } = new List<CobroDto>();
        public decimal MontoTotalCobrado { get; set; } = 0;

        //periodo
        public int Anio { get; set; }
        public int Mes { get; set; }
    }
}

namespace APIClub.Application.Dtos.Analiticas
{
    public class ResumenSociosDto
    {
        public int CantidadSociosActivos { get; set; }
        public int AltasEnPeriodo { get; set; }
        public int BajasEnPeriodo { get; set; }
        public decimal TasaMorosidad { get; set; }
    }
}

namespace APIClub.Application.Dtos.Cobrador
{
    public class CobroDto
    {
        public DateOnly FechaCobro { get; set; }
        public string NombreSocio { get; set; }
        public decimal MontoCobrado { get; set; }
    }
}

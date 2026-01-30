namespace APIClub.Application.Dtos.Analiticas
{
    public class ResumenIngresosDto
    {
        public decimal TotalGeneral { get; set; }
        public decimal TotalCuotas { get; set; }
        public decimal TotalAlquilerArticulos { get; set; }
        public decimal TotalReservasSalones { get; set; }
    }
}

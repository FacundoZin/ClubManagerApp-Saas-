namespace APIClub.Application.Dtos.Analiticas
{
    public class IngresoMensualDto
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public decimal Cuotas { get; set; }
        public decimal AlquilerArticulos { get; set; }
        public decimal ReservasSalones { get; set; }
        public decimal Total { get; set; }
    }
}

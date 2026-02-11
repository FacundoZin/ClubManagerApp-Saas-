namespace APIClub.Domain.ModuloGestionCobradores.Models
{
    public class RegistroCobrador
    {
        public int Id { get; set; }
        public DateOnly FechaCobro { get; set; }
        public string NombreSocio { get; set; } 
        public decimal MontoCobrado { get; set; }
        public int IdCobrador { get; set; }

    }
}

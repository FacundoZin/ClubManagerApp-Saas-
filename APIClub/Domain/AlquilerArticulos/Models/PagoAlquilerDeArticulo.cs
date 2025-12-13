namespace APIClub.Domain.AlquilerArticulos.Models
{
    public class PagoAlquilerDeArticulo
    {
        public int Id { get; set; }
        public DateOnly FechaPago { get; set; }
        public decimal Monto { get; set; }

        public int IdAlquiler { get; set; }
        public Alquiler alquiler { get; set; }
    }
}

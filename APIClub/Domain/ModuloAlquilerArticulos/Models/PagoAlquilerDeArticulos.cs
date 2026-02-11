namespace APIClub.Domain.AlquilerArticulos.Models
{
    public class PagoAlquilerDeArticulos
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int Monto { get; set; }

        public int IdAlquiler { get; set; }
        public Alquiler alquiler { get; set; }
    }
}

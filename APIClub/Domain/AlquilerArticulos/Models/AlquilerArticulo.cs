namespace APIClub.Domain.AlquilerArticulos.Models
{
    public class AlquilerArticulo
    {
        public int Id { get; set; }

        public int ArticuloId { get; set; }
        public Articulo Articulo { get; set; }

        public int AlquilerId { get; set; }
        public Alquiler Alquiler { get; set; }
    }
}

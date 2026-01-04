namespace APIClub.Application.Dtos.ItemsAlquiler
{
    public class DetailsItemAlquilerDto
    {
        public int Id { get; set; }
        public int ArticuloId { get; set; }
        public string NombreArticulo { get; set; }
        public int PrecioAlquiler { get; set; }
        public int Cantidad { get; set; }
        public int Subtotal { get; set; }
    }
}

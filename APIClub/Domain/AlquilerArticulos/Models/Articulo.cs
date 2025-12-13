using System.ComponentModel.DataAnnotations;

namespace APIClub.Domain.AlquilerArticulos.Models
{
    public class Articulo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioAlquiler { get; set; }
    }
}

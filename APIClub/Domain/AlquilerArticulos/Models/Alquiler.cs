using APIClub.Domain.GestionSocios.Models;

namespace APIClub.Domain.AlquilerArticulos.Models
{
    public class Alquiler
    {
        public int Id { get; set; }
        public DateOnly FechaAlquiler { get; set; }
        public string? Observaciones { get; set; }

        public int IdSocio { get; set; }
        public Socio Socio { get; set; }

        public List<AlquilerArticulo> Articulos { get; set; } = new List<AlquilerArticulo>();
        public List<PagoAlquilerDeArticulo> HistorialDePagos { get; set; } = new List<PagoAlquilerDeArticulo>();

        public bool Devuelto { get; set; } = false;
    }
}

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

        public List<ItemAlquiler> Items { get; set; } = new List<ItemAlquiler>();
        public List<PagoAlquilerDeArticulos> HistorialDePagos { get; set; } = new List<PagoAlquilerDeArticulos>();

        public bool Finalizado { get; set; } = false;
    }
}

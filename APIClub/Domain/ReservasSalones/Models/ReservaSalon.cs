using APIClub.Domain.GestionSocios.Models;

namespace APIClub.Domain.ReservasSalones.Models
{
    public class ReservaSalon
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateOnly FechaAlquiler { get; set; }
        public decimal Importe { get; set; }
        public decimal TotalPagado { get; set; } = 0;

        //Relaciones
        public int SocioId { get; set; }
        public Socio Socio { get; set; }

        public int SalonId { get; set; }
        public Salon Salon { get; set; }
    }
}

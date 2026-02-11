namespace APIClub.Domain.ReservasSalones.Models
{
    public class PagoReservaSalon
    {
        public int Id { get; set; }
        public DateOnly FechaPago { get; set; }
        public decimal monto { get; set; }
        
        // Foreign Key
        public int ReservaSalonId { get; set; }
    }
}

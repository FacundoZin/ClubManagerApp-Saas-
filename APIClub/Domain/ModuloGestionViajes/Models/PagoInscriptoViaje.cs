using System;

namespace APIClub.Domain.ModuloGestionViajes.Models
{
    public class PagoInscriptoViaje
    {
        public int Id { get; set; }
        public int InscriptoViajeId { get; set; }
        public decimal Monto { get; set; }
        public DateOnly FechaPago { get; set; }
        public string NumeroRecibo { get; set; } = string.Empty;
    }
}

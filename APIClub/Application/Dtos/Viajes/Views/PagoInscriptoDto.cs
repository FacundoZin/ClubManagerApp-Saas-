using System;

namespace APIClub.Application.Dtos.Viajes.Views
{
    public class PagoInscriptoDto
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateOnly FechaPago { get; set; }
        public string NumeroRecibo { get; set; } = string.Empty;
    }
}

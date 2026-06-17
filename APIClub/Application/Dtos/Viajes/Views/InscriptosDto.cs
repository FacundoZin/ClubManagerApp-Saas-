using System.Collections.Generic;

namespace APIClub.Application.Dtos.Viajes.Views
{
    public class InscriptosDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string NumeroFile { get; set; } = string.Empty;
        public decimal MontoAbonado { get; set; }
        public decimal MontoPendiente { get; set; }
        public bool Cancelado { get; set; }
        public List<PagoInscriptoDto> HistorialPagos { get; set; } = new();
        public List<PagoInscriptoModificacionDto> HistorialModificaciones { get; set; } = new();
    }
}

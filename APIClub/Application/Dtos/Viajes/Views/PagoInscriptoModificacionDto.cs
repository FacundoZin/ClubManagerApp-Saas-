using System;

namespace APIClub.Application.Dtos.Viajes.Views
{
    public class PagoInscriptoModificacionDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public decimal MontoAnterior { get; set; }
        public decimal MontoNuevo { get; set; }
        public decimal Diferencia { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }
}

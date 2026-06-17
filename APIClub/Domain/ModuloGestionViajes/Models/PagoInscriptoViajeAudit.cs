using System;

namespace APIClub.Domain.ModuloGestionViajes.Models
{
    public class PagoInscriptoViajeAudit
    {
        public int Id { get; set; }
        public int InscriptoViajeId { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public decimal MontoAnterior { get; set; }
        public decimal MontoNuevo { get; set; }
        public decimal Diferencia { get; set; }
        public string Motivo { get; set; } = string.Empty;

        public InscriptoViaje InscriptoViaje { get; set; } = null!;
    }
}

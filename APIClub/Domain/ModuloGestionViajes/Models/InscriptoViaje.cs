namespace APIClub.Domain.ModuloGestionViajes.Models
{
    public class InscriptoViaje
    {
        public int Id { get; set; }

        // Datos personales del inscripto (independientes de Socio)
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        // Número de file (agrupa inscriptos que viajan juntos)
        public string NumeroFile { get; set; } = string.Empty;

        // Relación con variante
        public int VarianteViajeId { get; set; }
        public VarianteViaje Variante { get; set; } = null!;

        // Metadata financiera
        public decimal MontoAbonado { get; set; }
        public decimal MontoPendiente { get; set; }
        public bool Cancelado { get; set; } = false;

        // Historial de pagos
        public List<PagoInscriptoViaje> HistorialPagos { get; set; } = new List<PagoInscriptoViaje>();

        // Auditoría de modificaciones de pagos
        public List<PagoInscriptoViajeAudit> HistorialModificaciones { get; set; } = new List<PagoInscriptoViajeAudit>();
    }
}

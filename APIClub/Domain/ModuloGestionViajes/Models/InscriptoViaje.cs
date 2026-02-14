using APIClub.Domain.GestionSocios.Models;

namespace APIClub.Domain.ModuloGestionViajes.Models
{
    public class InscriptoViaje
    {
        public int Id { get; set; }

        public int VarianteViajeId { get; set; } 
        public VarianteViaje Variante { get; set; } = null!;
        public bool cancelado { get; set; } = false;

        public int SocioId { get; set; }
        public Socio Socio { get; set; } = null!;

        //metadata 
        public decimal montoAbonado { get; set; }
        public decimal MontoPendiente { get; set; }
    }
}

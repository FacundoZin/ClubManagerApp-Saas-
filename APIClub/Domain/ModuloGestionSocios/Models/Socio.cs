using APIClub.Domain.GestionSocios.Validations;
using APIClub.Domain.Enums;
using APIClub.Domain.ModuloGestionCuotas.Models;
using APIClub.Domain.ModuloGestionCobradores.Models;

namespace APIClub.Domain.GestionSocios.Models
{
    public class Socio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }

        [PhoneNumer]
        public string? Telefono { get; set; }
        public string? Direcccion { get; set; }
        public string? Localidad { get; set; }
        public MetodosDePago PreferenciaDePago { get; set; }

        public DateOnly FechaAsociacion { get; set; }
        public bool IsActivo { get; set; } = true;
        public DateOnly? FechaDeBaja { get; set; }

        // Relaciones
        public int? LoteId { get; set; }
        public Lote? Lote { get; set; }
       
        public List<Cuota> HistorialCuotas { get; set; } = new List<Cuota>();
    }
}

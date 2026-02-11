using APIClub.Domain.Enums;

namespace APIClub.Application.Dtos.Socios
{
    public class PreviewSocioDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
        public string? Telefono { get; set; }
        public string? Direcccion { get; set; }
        public string? nombreLote { get; set; }
        public int? IdLote { get; set; }
        public string? Localidad { get; set; }
        public MetodosDePago PreferenciaDePago { get; set; }
        public DateOnly? FechaAsociacion { get; set; }

        public bool AdeudaCuotas { get; set; }

    }
}

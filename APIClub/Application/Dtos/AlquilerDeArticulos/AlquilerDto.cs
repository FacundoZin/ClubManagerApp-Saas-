using APIClub.Application.Dtos.ItemsAlquiler;

namespace APIClub.Application.Dtos.AlquilerDeArticulos
{
    public class AlquilerDto
    {
        public int Id { get; set; }
        public DateOnly FechaAlquiler { get; set; }
        public string? Observaciones { get; set; }
        public bool estaAlDia { get; set; }

        // Información del socio
        public int IdSocio { get; set; }
        public string NombreSocio { get; set; }
        public string ApellidoSocio { get; set; }
        public string DniSocio { get; set; }
        public string? TelefonoSocio { get; set; }
        public string? DireccionSocio {  get; set; }
        public string? LocalidadSocio { get; set; }   

        // Artículos alquilados
        public List<DetailsItemAlquilerDto> Items { get; set; } = new List<DetailsItemAlquilerDto>();

        // Pagos
        public List<PagoAlquilerDto> HistorialDePagos { get; set; } = new List<PagoAlquilerDto>();
        public List<PagoAlquilerAdeudadoDto> MesesAdeudados { get; set; } = new List<PagoAlquilerAdeudadoDto>();
    }

    public class PagoAlquilerAdeudadoDto
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int Monto { get; set; }
    }

    public class PagoAlquilerDto
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public int Mes {  get; set; }
        public int Monto { get; set; }
    }
}

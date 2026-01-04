namespace APIClub.Application.Dtos.AlquilerDeArticulos
{
    public class AlquilerPreviewDto
    {
        public int Id { get; set; }
        public DateOnly FechaAlquiler { get; set; }

        public string NombreSocio { get; set; }
        public string ApellidoSocio { get; set; }
        public string DniSocio { get; set; }
        public string? TelefonoSocio { get; set; }
        public string? DireccionSocio { get; set; }
        public string? LocalidadSocio { get; set; }

        public bool estaAlDia { get; set; }    
    }
}

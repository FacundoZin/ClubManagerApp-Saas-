namespace APIClub.Application.Dtos.AlquilerDeArticulos
{
    public class StatusAlquileresSociosDto
    {
        public int IdSocio { get; set; }
        public required string NombreSocio { get; set; }
        public required string ApellidoSocio { get; set; }
        public string? Telefono { get; set; }
        public required string Mensaje { get; set; }
        public int? IdAlquiler { get; set; }
    }
}

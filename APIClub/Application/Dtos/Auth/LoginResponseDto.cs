using APIClub.Domain.Enums;

namespace APIClub.Application.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string NombreUsuario { get; set; }
        public RolUsuario Rol { get; set; }
        public DateTime Expiracion { get; set; }
    }
}

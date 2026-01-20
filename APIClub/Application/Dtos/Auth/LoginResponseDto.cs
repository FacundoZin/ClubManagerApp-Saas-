using APIClub.Domain.Auth.Models;

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

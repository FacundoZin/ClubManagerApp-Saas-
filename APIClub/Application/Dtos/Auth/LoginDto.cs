using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; }
    }
}

using APIClub.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace APIClub.Application.Dtos.Auth
{
    public class CreateUsuarioDto
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        public RolUsuario Rol { get; set; }
    }
}

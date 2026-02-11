using APIClub.Application.Common;
using APIClub.Application.Dtos.Auth;

namespace APIClub.Domain.Auth.useCases
{
    public interface IUsuariosService
    {
        Task<Result<UsuarioDto>> CreateUsuario(CreateUsuarioDto dto, int requestingUserId);
        Task<Result<List<UsuarioDto>>> GetAllUsuarios(int requestingUserId);
    }
}

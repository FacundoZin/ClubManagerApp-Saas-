using APIClub.Application.Common;
using APIClub.Application.Dtos.Auth;

namespace APIClub.Domain.Auth.useCases
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> Login(LoginDto dto);
        Task<Result<UsuarioDto>> GetCurrentUser(int userId);
    }
}

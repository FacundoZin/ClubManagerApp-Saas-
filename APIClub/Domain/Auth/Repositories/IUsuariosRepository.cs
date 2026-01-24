using APIClub.Application.Common;
using APIClub.Domain.Auth.Models;

namespace APIClub.Domain.Auth.Repositories
{
    public interface IUsuariosRepository
    {
        Task<Usuario?> GetByNombreUsuario(string nombreUsuario);
        Task<Usuario?> GetById(int id);
        Task<List<Usuario>> GetAll();
        Task<Usuario> Create(Usuario usuario);
        Task<bool> ExisteNombreUsuario(string nombreUsuario);
        Task ActualizarUltimoAcceso(int id);
        Task<List<Usuario>> GetUsuariosCobradores();  
    }
}

using APIClub.Domain.Auth.Models;
using APIClub.Domain.Auth.Repositories;
using APIClub.Domain.Enums;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Infrastructure.Persistence.Repositorio
{
    public class UsuariosRepository : IUsuariosRepository
    {
        private readonly AppDbcontext _context;

        public UsuariosRepository(AppDbcontext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByNombreUsuario(string nombreUsuario)
        {
            return await _context.Usuarios.AsNoTracking()
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
        }

        public async Task<Usuario?> GetById(int id)
        {
            return await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Usuario>> GetAll()
        {
            return await _context.Usuarios.AsNoTracking().ToListAsync();
        }

        public async Task<Usuario> Create(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> ExisteNombreUsuario(string nombreUsuario)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == nombreUsuario);
        }

        public async Task ActualizarUltimoAcceso(int id)
        {
            await _context.Usuarios
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(
                    u => u.UltimoAcceso,
                    DateTime.UtcNow
                ));
        }

        public async Task<List<Usuario>> GetUsuariosCobradores()
        {
            return await _context.Usuarios.AsNoTracking().Where(u => u.Rol == RolUsuario.Cobrador).ToListAsync();
        }
    }
}

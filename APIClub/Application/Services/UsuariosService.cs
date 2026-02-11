using APIClub.Application.Common;
using APIClub.Application.Dtos.Auth;
using APIClub.Domain.Auth.Models;
using APIClub.Domain.Auth.Repositories;
using APIClub.Domain.Auth.useCases;
using APIClub.Domain.Enums;

namespace APIClub.Application.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuariosRepository _usuariosRepository;

        public UsuariosService(IUsuariosRepository usuariosRepository)
        {
            _usuariosRepository = usuariosRepository;
        }

        public async Task<Result<UsuarioDto>> CreateUsuario(CreateUsuarioDto dto, int requestingUserId)
        {
            // Verificar que el usuario solicitante es SuperAdmin
            var requestingUser = await _usuariosRepository.GetById(requestingUserId);
            
            if (requestingUser == null || requestingUser.Rol != RolUsuario.SuperAdmin)
                return Result<UsuarioDto>.Error("Solo los administradores pueden crear usuarios", 403);

            // Verificar que no exista el nombre de usuario
            if (await _usuariosRepository.ExisteNombreUsuario(dto.NombreUsuario))
                return Result<UsuarioDto>.Error("El nombre de usuario ya existe", 409);

            // Hashear contraseña con BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var nuevoUsuario = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                PasswordHash = passwordHash,
                Rol = dto.Rol,
                FechaCreacion = DateTime.UtcNow,
                UltimoAcceso = null
            };

            var usuarioCreado = await _usuariosRepository.Create(nuevoUsuario);

            return Result<UsuarioDto>.Exito(new UsuarioDto
            {
                Id = usuarioCreado.Id,
                NombreUsuario = usuarioCreado.NombreUsuario,
                Rol = usuarioCreado.Rol,
                FechaCreacion = usuarioCreado.FechaCreacion,
                UltimoAcceso = usuarioCreado.UltimoAcceso
            });
        }

        public async Task<Result<List<UsuarioDto>>> GetAllUsuarios(int requestingUserId)
        {
            // Verificar que el usuario solicitante es SuperAdmin
            var requestingUser = await _usuariosRepository.GetById(requestingUserId);
            
            if (requestingUser == null || requestingUser.Rol != RolUsuario.SuperAdmin)
                return Result<List<UsuarioDto>>.Error("No tiene permisos para ver usuarios", 403);

            var usuarios = await _usuariosRepository.GetAll();

            var usuariosDto = usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                NombreUsuario = u.NombreUsuario,
                Rol = u.Rol,
                FechaCreacion = u.FechaCreacion,
                UltimoAcceso = u.UltimoAcceso
            }).ToList();

            return Result<List<UsuarioDto>>.Exito(usuariosDto);
        }
    }
}

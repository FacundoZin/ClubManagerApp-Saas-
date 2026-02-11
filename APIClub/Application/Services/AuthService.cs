using APIClub.Application.Common;
using APIClub.Application.Dtos.Auth;
using APIClub.Domain.Auth.Models;
using APIClub.Domain.Auth.Repositories;
using APIClub.Domain.Auth.useCases;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIClub.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUsuariosRepository usuariosRepository, IConfiguration configuration)
        {
            _usuariosRepository = usuariosRepository;
            _configuration = configuration;
        }

        public async Task<Result<LoginResponseDto>> Login(LoginDto dto)
        {
            var usuario = await _usuariosRepository.GetByNombreUsuario(dto.NombreUsuario);

            if (usuario == null)
                return Result<LoginResponseDto>.Error("Credenciales inválidas", 401);

            // Verificar contraseña con BCrypt
            bool passwordValido = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);

            if (!passwordValido)
                return Result<LoginResponseDto>.Error("Credenciales inválidas", 401);

            // Actualizar último acceso
            await _usuariosRepository.ActualizarUltimoAcceso(usuario.Id);

            // Generar JWT token
            var token = GenerarJwtToken(usuario);
            var expiracion = DateTime.UtcNow.AddHours(336); // 2 semanas

            return Result<LoginResponseDto>.Exito(new LoginResponseDto
            {
                Token = token,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                Expiracion = expiracion
            });
        }

        public async Task<Result<UsuarioDto>> GetCurrentUser(int userId)
        {
            var usuario = await _usuariosRepository.GetById(userId);

            if (usuario == null)
            {
            {
                return Result<UsuarioDto>.Error("Usuario no encontrado", 404);
            }
            }

            return Result<UsuarioDto>.Exito(new UsuarioDto
            {
                Id = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                FechaCreacion = usuario.FechaCreacion,
                UltimoAcceso = usuario.UltimoAcceso
            });
        }

        private string GenerarJwtToken(Usuario usuario)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(336), // 2 semanas
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

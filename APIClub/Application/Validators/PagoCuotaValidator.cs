using APIClub.Application.Common;
using APIClub.Domain.Auth.Models;
using APIClub.Domain.Auth.Repositories;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.GestionSocios.Validations;

namespace APIClub.Application.Validators
{
    public class PagoCuotaValidator : IPagoCuotaValidator
    {
        private ISocioRepository _socioRepository;
        private IUsuariosRepository _usuariosRepository;
        
        public PagoCuotaValidator(ISocioRepository socioRepository, IUsuariosRepository usuariosRepository)
        {
            _socioRepository = socioRepository;
            _usuariosRepository = usuariosRepository;   
        }

        public async Task<Result<Socio>> ValidarPagoConCobrador(int idSocio, int idUsuarioSistema)
        {
            var usuario = await _usuariosRepository.GetById(idUsuarioSistema);

            if (usuario.Rol != RolUsuario.Cobrador)
                return Result<Socio>.Error("solamente los cobradores pueden registrar este tipo de pagos", 403);

            var socio = await _socioRepository.GetSocioById(idSocio);
            if (socio == null)
                return Result<Socio>.Error("Socio no encontrado.", 404);

            var fechaPago = DateOnly.FromDateTime(DateTime.Now);
            int anio = fechaPago.Year;
            int semestre = fechaPago.Month <= 6 ? 1 : 2;

            bool cuotaExistente = socio.HistorialCuotas.Any(c =>
                c.Anio == anio && c.Semestre == semestre);

            if (cuotaExistente)
                return Result<Socio>.Error("Ya existe una cuota registrada en el periodo actual", 409);

            return Result<Socio>.Exito(socio);
        }

        public async Task<Result<Socio>> ValidarPagoEnEstablecimeinto(int idSocio)
        {
            var socio = await _socioRepository.GetSocioById(idSocio);
            if (socio == null)
                return Result<Socio>.Error("Socio no encontrado.", 404);

            var fechaPago = DateOnly.FromDateTime(DateTime.Now);
            int anio = fechaPago.Year;
            int semestre = fechaPago.Month <= 6 ? 1 : 2;

            bool cuotaExistente = socio.HistorialCuotas.Any(c =>
                c.Anio == anio && c.Semestre == semestre);

            if (cuotaExistente)
                return Result<Socio>.Error("Ya existe una cuota registrada en el periodo actual", 409);

            return Result<Socio>.Exito(socio);
        }
    }
}

using APIClub.Application.Common;
using APIClub.Domain.Enums;
using APIClub.Domain.GestionSocios;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.GestionSocios.Validations;
using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Application.Services
{
    public class CuotasService : ICuotasService
    {
        private readonly ISocioRepository _SocioRepository;
        private readonly ICuotaRepository _CuotaRepository;
        private readonly IHistorialCobradoresRepository _HistorialCobradoresRepository;
        private readonly IPagoCuotaValidator _Validator;
        private readonly UnitOfWork _UnitOfWork;

        public CuotasService(ISocioRepository socioRepository, ICuotaRepository cuotaRepository, IPagoCuotaValidator validator, IHistorialCobradoresRepository historialCobradoresRepository,
            UnitOfWork unitOfWork)
        {
            _SocioRepository = socioRepository;
            _CuotaRepository = cuotaRepository;
            _Validator = validator;
            _HistorialCobradoresRepository = historialCobradoresRepository;
            _UnitOfWork = unitOfWork;
        }

        public async Task<Result<object>> ActualizarValorCuota(decimal nuevoValor)
        {
            var fechaActualizacion = DateTime.Now;
            await _CuotaRepository.ActualizarValorCuota(nuevoValor, fechaActualizacion);
            return Result<object>.Exito("al valor de la cuoata ahora es $" + nuevoValor);
        }

        public async Task<Result<object>> RegistrarPagoCuoata(int idSocio, FormasDePago formaPago)
        {
            var result = await _Validator.ValidarPagoEnEstablecimeinto(idSocio);

            if (!result.Exit) return Result<object>.Error(result.Errormessage, result.Errorcode);
            
            var socio = result.Data;
            var now = DateOnly.FromDateTime(DateTime.Now);
            var valorCuotaActual = await _CuotaRepository.ObtenerValorCuota();

            var nuevaCuota = new Cuota
            {
                FechaPago = now,
                Monto = valorCuotaActual,
                FormaDePago = formaPago,
                Anio = now.Year,
                Semestre = now.Month < 7 ? 1 : 2,
                SocioId = socio.Id,
                Socio = socio
            };

            socio.HistorialCuotas.Add(nuevaCuota);
            await _SocioRepository.UpdateSocio(socio);

            return Result<object>.Exito(new
            {
                Mensaje = "Pago de cuota registrado exitosamente.",
                Cuota = nuevaCuota
            });
        }

        public async Task<Result<object?>> RegistrarPagoCuoataCobrador(int idSocio, int idUsuario)
        {
            var validationResult = await _Validator.ValidarPagoConCobrador(idSocio,idUsuario);

            if (!validationResult.Exit) return Result<object?>.Error(validationResult.Errormessage, validationResult.Errorcode);

            var socio = validationResult.Data;
            var now = DateOnly.FromDateTime(DateTime.Now);
            var valorCuotaActual = await _CuotaRepository.ObtenerValorCuota();

            var cobro = new RegistroCobrador { 
                FechaCobro = now, 
                IdCobrador = idUsuario, 
                MontoCobrado = valorCuotaActual, 
                NombreSocio = socio.Nombre 
            };

            var nuevaCuota = new Cuota
            {
                FechaPago = now,
                Monto = valorCuotaActual,
                FormaDePago = FormasDePago.Cobrador,
                Anio = now.Year,
                Semestre = now.Month < 7 ? 1 : 2,
                SocioId = socio.Id,
                Socio = socio
            };

            socio.HistorialCuotas.Add(nuevaCuota);

            _HistorialCobradoresRepository.AddCobroToHistorial(cobro);
            _SocioRepository.UpdateSocioWhitoutSave(socio);

            var exit = await _UnitOfWork.SaveChangesAsync();

            if (exit == false) return Result<object?>.Error("algo salio mal al registrar el pago", 500);

            return Result<object?>.Exito(new
            {
                Mensaje = "Pago de cuota registrado exitosamente.",
                Cuota = nuevaCuota
            });
        }

        public async Task<Result<object>> RegistrarPagoCuoataOnline(PaymentToken token)
        {
            var socio = await _SocioRepository.GetSocioById(token.IdSocio);
            if (socio == null)
                return Result<object>.Error("Socio no encontrado.", 404);

            var fechaPago = DateOnly.FromDateTime(DateTime.Now);


            bool cuotaExistente = socio.HistorialCuotas.Any(c =>
                c.Anio == token.anio && c.Semestre == token.semestre);

            if (cuotaExistente)
                return Result<object>.Error("Ya existe una cuota registrada para este semestre y año.", 409);

            var valorCuotaActual = await _CuotaRepository.ObtenerValorCuota();

            var nuevaCuota = new Cuota
            {
                FechaPago = fechaPago,
                Monto = valorCuotaActual,
                FormaDePago = FormasDePago.LinkDePago,
                Anio = token.anio,
                Semestre = token.semestre,
                SocioId = socio.Id,
                Socio = socio
            };

            socio.HistorialCuotas.Add(nuevaCuota);
            await _SocioRepository.UpdateSocio(socio);

            return Result<object>.Exito(new
            {
                Mensaje = "Pago de cuota registrado exitosamente.",
                Cuota = nuevaCuota
            });

        }
    }
}

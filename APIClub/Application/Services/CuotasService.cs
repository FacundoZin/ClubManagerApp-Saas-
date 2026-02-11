using APIClub.Application.Common;
using APIClub.Application.Dtos.Cuota;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.Enums;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.ModuloGestionCobradores.Models;
using APIClub.Domain.ModuloGestionCobradores.Repositorios;
using APIClub.Domain.ModuloGestionCuotas.Models;
using APIClub.Domain.ModuloGestionCuotas.UseCases;
using APIClub.Domain.ModuloGestionCuotas.Validations;
using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Application.Services
{
    public class CuotasService : ICuotasService
    {
        private readonly ICuotaRepository _CuotaRepository;
        private readonly IHistorialCobradoresRepository _HistorialCobradoresRepository;
        private readonly IPagoCuotaValidator _Validator;
        private readonly UnitOfWork _UnitOfWork;

        public CuotasService(ICuotaRepository cuotaRepository, IPagoCuotaValidator validator, IHistorialCobradoresRepository historialCobradoresRepository,
            UnitOfWork unitOfWork)
        {
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

        public async Task<Result<object>> RegistrarPagosCuotas(int idSocio, List<PeriodoAdeudadoDto> periodos, MetodosDePago formaPago)
        {
            var result = await _Validator.ValidarPagoEnEstablecimeinto(idSocio, periodos);

            if (!result.Exit) return Result<object>.Error(result.Errormessage, result.Errorcode);

            var socio = result.Data;
            var now = DateOnly.FromDateTime(DateTime.Now);
            var valorCuotaActual = await _CuotaRepository.ObtenerValorCuota();

            var nuevasCuotas = new List<Cuota>();

            foreach (var periodo in periodos)
            {
                var nuevaCuota = new Cuota
                {
                    FechaPago = now,
                    Monto = valorCuotaActual,
                    FormaDePago = formaPago,
                    Anio = periodo.Anio,
                    Semestre = periodo.Semestre,
                    SocioId = socio.Id
                };

                nuevasCuotas.Add(nuevaCuota);
            }

            if (nuevasCuotas.Count == 0)
            {
                return Result<object>.Error("No se seleccionaron periodos para pagar.", 400);
            }

            _CuotaRepository.RegistrarCuotas(nuevasCuotas);
            await _UnitOfWork.SaveChangesAsync();

            return Result<object>.Exito(new
            {
                Mensaje = "Pagos de cuota registrados exitosamente."
            });
        }

        public async Task<Result<object?>> RegistrarPagosCuotasCobrador(int idSocio, List<PeriodoAdeudadoDto> periodos, int idUsuario)
        {
            var validationResult = await _Validator.ValidarPagoConCobrador(idSocio, idUsuario, periodos);

            if (!validationResult.Exit) return Result<object?>.Error(validationResult.Errormessage, validationResult.Errorcode);

            var socio = validationResult.Data;
            var now = DateOnly.FromDateTime(DateTime.Now);
            var valorCuotaActual = await _CuotaRepository.ObtenerValorCuota();

            var nuevasCuotas = new List<Cuota>();

            foreach (var periodo in periodos)
            {
                var cobro = new RegistroCobrador
                {
                    FechaCobro = now,
                    IdCobrador = idUsuario,
                    MontoCobrado = valorCuotaActual,
                    NombreSocio = socio.Nombre
                };

                var nuevaCuota = new Cuota
                {
                    FechaPago = now,
                    Monto = valorCuotaActual,
                    FormaDePago = MetodosDePago.Cobrador,
                    Anio = periodo.Anio,
                    Semestre = periodo.Semestre,
                    SocioId = socio.Id
                };

                nuevasCuotas.Add(nuevaCuota);
                _HistorialCobradoresRepository.AddCobroToHistorial(cobro);
            }

            _CuotaRepository.RegistrarCuotas(nuevasCuotas);

            var exit = await _UnitOfWork.SaveChangesAsync();

            if (exit == false) return Result<object?>.Error("algo salio mal al registrar el pago", 500);

            return Result<object?>.Exito(new
            {
                Mensaje = $"Pago de cuota registrado exitosamente.",
            });
        }

        public async Task<Result<object>> RegistrarPagoCuoataOnline(PaymentToken token)
        {
            var fechaPago = DateOnly.FromDateTime(DateTime.Now);
            var valorCuotaActual = await _CuotaRepository.ObtenerValorCuota();

            var nuevaCuota = new Cuota
            {
                FechaPago = fechaPago,
                Monto = valorCuotaActual,
                FormaDePago = MetodosDePago.LinkDePago,
                Anio = token.anio,
                Semestre = token.semestre,
                SocioId = token.IdSocio
            };

            _CuotaRepository.RegistrarCuotas(new List<Cuota> { nuevaCuota });
            await _UnitOfWork.SaveChangesAsync();

            return Result<object>.Exito(new
            {
                Mensaje = "Pago de cuota registrado exitosamente.",
                Cuota = nuevaCuota
            });
        }

        public async Task<Result<PagedResult<CuotaConSocioDto>>> VerHistorialCuotas(HistorialCuotasRequestDto request)
        {
            PagedResult<CuotaConSocioDto> resultado;

            if (request.TipoFiltro.ToLower() == "fecha")
            {
                if (!request.FechaPago.HasValue)
                {
                    return Result<PagedResult<CuotaConSocioDto>>.Error("Debe proporcionar una fecha de pago para filtrar por fecha.", 400);
                }

                resultado = await _CuotaRepository.ObtenerCuotasPorFechaPago(
                    request.FechaPago.Value,
                    request.PageNumber,
                    request.PageSize
                );
            }
            else if (request.TipoFiltro.ToLower() == "periodo")
            {
                if (!request.Anio.HasValue || !request.Semestre.HasValue)
                {
                    return Result<PagedResult<CuotaConSocioDto>>.Error("Debe proporcionar año y semestre para filtrar por periodo.", 400);
                }

                resultado = await _CuotaRepository.ObtenerCuotasPorPeriodo(
                    request.Anio.Value,
                    request.Semestre.Value,
                    request.PageNumber,
                    request.PageSize
                );
            }
            else
            {
                return Result<PagedResult<CuotaConSocioDto>>.Error("Tipo de filtro inválido. Use 'fecha' o 'periodo'.", 400);
            }

            return Result<PagedResult<CuotaConSocioDto>>.Exito(resultado);
        }
    }
}

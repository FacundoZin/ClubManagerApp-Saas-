using APIClub.Application.Common;
using APIClub.Domain.Enums;
using APIClub.Domain.GestionSocios;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Application.Services
{
    public class CuotasService : ICuotasService
    {
        private readonly ISocioRepository _SocioRepository;
        private readonly ICuotaRepository _CuotaRepository;
        public CuotasService(ISocioRepository socioRepository, ICuotaRepository cuotaRepository) 
        {
            _SocioRepository = socioRepository; 
            _CuotaRepository = cuotaRepository;
        }

        public async Task<Result<object>> ActualizarValorCuota(decimal nuevoValor)
        {
            var fechaActualizacion = DateTime.Now;
            await _CuotaRepository.ActualizarValorCuota(nuevoValor, fechaActualizacion);
            return Result<object>.Exito("al valor de la cuoata ahora es $" + nuevoValor);
        }

        public async Task<Result<object>> RegistrarPagoCuoata(int idSocio, FormasDePago formaPago)
        {
            var socio = await _SocioRepository.GetSocioById(idSocio);
            if (socio == null)
                return Result<object>.Error("Socio no encontrado.", 404);

            var fechaPago = DateOnly.FromDateTime(DateTime.Now);
            int anio = fechaPago.Year;
            int semestre = fechaPago.Month <= 6 ? 1 : 2;

            bool cuotaExistente = socio.HistorialCuotas.Any(c =>
                c.Anio == anio && c.Semestre == semestre);

            if (cuotaExistente)
                return Result<object>.Error("Ya existe una cuota registrada para este semestre y año.", 409);

            var valorCuotaActual = await _CuotaRepository.ObtenerValorCuota();

            var nuevaCuota = new Cuota
            {
                FechaPago = fechaPago,
                Monto = valorCuotaActual,
                FormaDePago = formaPago,
                Anio = anio,
                Semestre = semestre,
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

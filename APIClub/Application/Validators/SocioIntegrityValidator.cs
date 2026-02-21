using APIClub.Application.Common;
using APIClub.Application.Dtos.Socios;
using APIClub.Application.Helpers;
using APIClub.Domain.Enums;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.GestionSocios.Validations;

namespace APIClub.Application.Validators
{
    public class SocioIntegrityValidator : ISocioIntegrityValidator
    {
        private readonly UnitOfWork _unitOfWork;

        public SocioIntegrityValidator(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<object?>> ValidateCargaSocio(CreateSocioDto dto)
        {
            var existingSocio = await _unitOfWork._SocioRepository.GetSocioByDniIgnoreFilter(dto.Dni);

            if (existingSocio != null)
            {
                if (existingSocio.IsActivo)
                {
                    return Result<object?>.Error("El socio que quiere cargar ya está activo.", 400);
                }
                else
                {
                    var data = new ExistingSocio
                    {
                        Id = existingSocio.Id,
                        Nombre = existingSocio.Nombre + " " + existingSocio.Apellido
                    };
                    return Result<object?>.Conflict("El socio ya existe pero está dado de baja.", 409, data);
                }
            }

            var telefonoFormateado = string.Empty;

            if (!string.IsNullOrEmpty(dto.Telefono))
            {
                var ResultFormateo = dto.Telefono.FormatearNumero();
                telefonoFormateado = ResultFormateo.Data;

                if(!ResultFormateo.Exit)
                    return Result<object?>.Error(ResultFormateo.Errormessage, ResultFormateo.Errorcode);
            }

            if (dto.PreferenciaDePago == MetodosDePago.LinkDePago)
            {
                if (string.IsNullOrEmpty(dto.Telefono)) return Result<object?>
                        .Error("si el socio va a pagar con link de pago es necesario que ingrese un numero de telefono", 400);

                if (telefonoFormateado.EsTelefonoFijo())
                    return Result<object?>
                        .Error("si el socio va a pagar con link de pago no puede ingresar un telefono fijo, cambie el numero de telefono o elija otra preferencia de pago", 400);
            }

            if (dto.PreferenciaDePago == MetodosDePago.Cobrador && dto.IdLote == null)
                return Result<object?>.Error("si elsocio va a pagar con cobrador es necesario asignarle un lote", 400);

            return Result<object?>.Exito(null);
        }

        public async Task<Result<bool>> ValidateSocioDeletion(int socioId)
        {
            // 1. Verificar Alquileres Activos
            var hasActiveAlquileres = await _unitOfWork._AlquilerRepository.HasActiveAlquilerBySocio(socioId);
            if (hasActiveAlquileres)
            {
                return Result<bool>.Error("No se puede dar de baja al socio porque posee alquileres de artículos activos (no finalizados).", 400);
            }

            // 2. Verificar Reservas Futuras
            var hasFutureReservas = await _unitOfWork._ReservasRepository.HasFutureReservationsBySocio(socioId);
            if (hasFutureReservas)
            {
                return Result<bool>.Error("No se puede dar de baja al socio porque tiene una reserva de salon activa, " +
                    "cancele la reserva si quiere dar de baja al socio", 400);
            }

            // Si pasa todas las validaciones
            return Result<bool>.Exito(true);
        }

        public async Task<Result<Socio>> ValidateUpdateSocio(int id, UpdateSocio dto)
        {
            if (id <= 0)
            {
                return Result<Socio>.Error("El ID proporcionado no es válido.", 400);
            }

            var socio = await _unitOfWork._SocioRepository.GetSocioById(id);

            if (socio is null)
            {
                return Result<Socio>.Error("No se encontró un socio con ese ID.", 404);
            }

            var telefonoFormateado = string.Empty;

            if (!string.IsNullOrEmpty(dto.Telefono))
            {
                var ResultFormateo = dto.Telefono.FormatearNumero();
                telefonoFormateado = ResultFormateo.Data;

                if (!ResultFormateo.Exit)
                    return Result<Socio>.Error(ResultFormateo.Errormessage, ResultFormateo.Errorcode);
            }

            if (dto.PreferenciaDePago == MetodosDePago.LinkDePago)
            {
                if (string.IsNullOrEmpty(dto.Telefono)) return Result<Socio>
                        .Error("si el socio va a pagar con link de pago es necesario que ingrese un numero de telefono", 400);

                if (telefonoFormateado.EsTelefonoFijo())
                    return Result<Socio>
                        .Error("si el socio va a pagar con link de pago no puede ingresar un telefono fijo, cambie el numero de telefono o elija otra preferencia de pago", 400);
            }

            if (dto.PreferenciaDePago == MetodosDePago.Cobrador && dto.IdLote == null)
                return Result<Socio>.Error("si el socio quiere pagar con cobrador es necesario asignarle un lote", 400);

            // Validar que el nuevo DNI no esté asignado a otro socio
            if (socio.Dni != dto.Dni)
            {
                var dniExists = await _unitOfWork._SocioRepository.SocioExists(dto.Dni);
                if (dniExists)
                {
                    return Result<Socio>.Error("Ya existe un socio con ese DNI.", 400);
                }
            }

            return Result<Socio>.Exito(socio);
        }
    }
}

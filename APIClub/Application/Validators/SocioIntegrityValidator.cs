using APIClub.Application.Common;
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
    }
}

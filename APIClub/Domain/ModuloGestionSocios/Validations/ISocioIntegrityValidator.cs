using APIClub.Application.Common;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.GestionSocios.Models;

namespace APIClub.Domain.GestionSocios.Validations
{
    public interface ISocioIntegrityValidator
    {
        Task<Result<bool>> ValidateSocioDeletion(int socioId);
        Task<Result<Socio>> ValidateUpdateSocio(int id, UpdateSocio dto);
        Task<Result<object?>> ValidateCargaSocio(CreateSocioDto dto);
    }
}

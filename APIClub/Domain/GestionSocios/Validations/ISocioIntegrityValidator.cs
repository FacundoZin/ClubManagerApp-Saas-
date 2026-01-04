using APIClub.Application.Common;

namespace APIClub.Domain.GestionSocios.Validations
{
    public interface ISocioIntegrityValidator
    {
        Task<Result<bool>> ValidateSocioDeletion(int socioId);
    }
}

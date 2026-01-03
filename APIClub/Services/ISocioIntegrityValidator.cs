using APIClub.Common;

namespace APIClub.Services
{
    public interface ISocioIntegrityValidator
    {
        Task<Result<bool>> ValidateSocioDeletion(int socioId);
    }
}

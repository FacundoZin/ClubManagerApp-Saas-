using APIClub.Application.Common;
using APIClub.Application.Dtos.Payment;
using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Domain.PaymentsOnline.useCases
{
    public interface IPaymentTokenService
    {
        Task<Result<PaymentToken>> ValidateToken(Guid idToken);
        Task<Result<object?>> ExpireToken(Guid idToken);
        Task CreatePaymentTokens();
    }
}

using APIClub.Common;
using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Dtos.Payment;

namespace APIClub.Domain.PaymentsOnline
{
    public interface IPaymentTokenService
    {
        Task<Result<PaymentToken>> ValidateToken(Guid idToken);
        Task<Result<object?>> ExpireToken(Guid idToken);
        Task<string> CreateToken(CreateTokenRequestDto dto);
    }
}

using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Domain.PaymentsOnline.Repository
{
    public interface IPaymentTokenRepository
    {
        Task CreateTokens(List<PaymentToken> token);
        Task<PaymentToken?> GetToken(Guid tokenId);
        Task<bool> confirmPaymentIfNotWereUsed(Guid tokenId);
    }
}

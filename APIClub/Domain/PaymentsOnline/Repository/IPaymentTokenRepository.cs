using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Domain.PaymentsOnline.Repository
{
    public interface IPaymentTokenRepository
    {
        Task<bool> CreateToken(PaymentToken token);
        Task<PaymentToken?> GetToken(Guid tokenId);
        Task<bool> confirmPaymentIfNotWereUsed(Guid tokenId);
    }
}

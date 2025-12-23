using APIClub.Common;
using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Dtos.Payment;
using Microsoft.AspNetCore.Components.Web;

namespace APIClub.Domain.PaymentsOnline.Repository
{
    public interface IPaymentTokenRepository
    {
        Task<bool> CreateToken(PaymentToken token);
        Task<PaymentToken?> GetToken(Guid tokenId);
    }
}

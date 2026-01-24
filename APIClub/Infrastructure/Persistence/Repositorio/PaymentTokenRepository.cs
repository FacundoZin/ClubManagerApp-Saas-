using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Domain.PaymentsOnline.Repository;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace APIClub.Infrastructure.Persistence.Repositorio
{
    public class PaymentTokenRepository : IPaymentTokenRepository
    {
        private readonly AppDbcontext _DBcontext;

        public PaymentTokenRepository (AppDbcontext dbcontext)
        {
            _DBcontext = dbcontext;
        }

        public async Task CreateTokens(List<PaymentToken> token)
        {
            _DBcontext.PaymentTokens.AddRange(token);
            await _DBcontext.SaveChangesAsync();
        }

        public async Task<PaymentToken?> GetToken(Guid tokenId)
        {
            return await _DBcontext.PaymentTokens.FirstOrDefaultAsync(t => t.Id == tokenId);
        }

        public async Task<bool> confirmPaymentIfNotWereUsed(Guid tokenId)
        {
            var rowsAffected = await _DBcontext.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE PaymentTokens
                SET Usado = 1, PaymentStatus = 'approved'
                WHERE Id = {tokenId}
                AND Usado = 0");

            return rowsAffected == 1;
        }

        public async Task<bool> UpdatePaymentStatusIfNotFinal(Guid tokenId, string paymentStatus, string? statusDetail)
        {
            var rowsAffected = await _DBcontext.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE PaymentTokens
                SET PaymentStatus = {paymentStatus}, StatusDetail = {statusDetail}
                WHERE Id = {tokenId}
                AND usado = 0
                AND (PaymentStatus IS NULL OR (PaymentStatus <> 'approved' AND PaymentStatus <> 'rejected'))");

            return rowsAffected == 1;
        }

    }
}

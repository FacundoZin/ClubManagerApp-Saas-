using APIClub.Common;
using APIClub.Data;
using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Domain.PaymentsOnline.Repository;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Repositorio
{
    public class PaymentTokenRepository : IPaymentTokenRepository
    {
        private readonly AppDbcontext _DBcontext;

        public PaymentTokenRepository (AppDbcontext dbcontext)
        {
            _DBcontext = dbcontext;
        }

        public async Task<bool> CreateToken(PaymentToken token)
        {
            _DBcontext.PaymentTokens.Add(token);
            int rowsAffected = await _DBcontext.SaveChangesAsync();

            return rowsAffected > 0;
        }

        public async Task<PaymentToken?> GetToken(Guid tokenId)
        {
            return await _DBcontext.PaymentTokens.FirstOrDefaultAsync(t => t.Id == tokenId);
        }

        public async Task<bool> confirmPaymentIfNotWereUsed(Guid tokenId)
        {
            var rowsAffected = await _DBcontext.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE PaymentTokens
                SET Usado = 1, PaymentStatus = {(int)PaymentStatus.Confirmed}
                WHERE Id = {tokenId}
                AND Usado = 0");

            return rowsAffected == 1;
        }

    }
}

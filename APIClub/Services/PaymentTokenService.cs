using APIClub.Common;
using APIClub.Domain.PaymentsOnline;
using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Dtos.Payment;

namespace APIClub.Services
{
    public class PaymentTokenService : IPaymentTokenService
    {
        private readonly UnitOfWork _UnitOfWork;

        public PaymentTokenService(UnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        public async Task<string> CreateToken(CreateTokenRequestDto dto)
        {
            var fechaExpiracion = DateOnly.FromDateTime(DateTime.Now).AddDays(30);

            PaymentToken paymentToken = new PaymentToken
            {
                Id = Guid.NewGuid(),
                IdSocio = dto.IdSocio,
                anio = dto.anio,
                semestre = dto.semestre,
                FechaExpiracion = fechaExpiracion,
                monto = dto.monto,
                usado = false,
            };

            await _UnitOfWork._PaymentTokenRepository.CreateToken(paymentToken);

            return paymentToken.Id.ToString(); 
        }

        public async Task<Result<object?>> ExpireToken(Guid idToken)
        {
            var token = await _UnitOfWork._PaymentTokenRepository.GetToken(idToken);

            if(token == null) return Result<object?>.Error("el token a expirar no existe", 404);
            if(token.usado) return Result<object?>.Error("EL TOKEN YA FUE USADO",422 );

            token.usado = true;

            bool exit = await _UnitOfWork.SaveChangesAsync();

            if (!exit) return Result<object?>.Error("algo salio mal al moficiar el token en la base de datos", 500);

            return Result<object?>.Exito(null);
        }

        public async Task<Result<PaymentToken>> ValidateToken(Guid idToken)
        {
            var date = DateTime.Now;
            var hoy = DateOnly.FromDateTime(date);


            var token = await _UnitOfWork._PaymentTokenRepository.GetToken(idToken);

            if (token == null) return Result<PaymentToken>.Error("el token no existe", 404);
            if (token.usado) return  Result<PaymentToken>.Error("el token ya fue utilizado", 422);
            if (hoy > token.FechaExpiracion) return Result<PaymentToken>.Error("el token ya no es valido", 422);

            return Result<PaymentToken>.Exito(token);
        }
    }
}

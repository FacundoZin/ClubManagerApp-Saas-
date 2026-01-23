using APIClub.Application.Common;
using APIClub.Domain.PaymentsOnline;
using APIClub.Domain.PaymentsOnline.Modelos;

namespace APIClub.Application.Services
{
    public class PaymentTokenService : IPaymentTokenService
    {
        private readonly UnitOfWork _UnitOfWork;

        public PaymentTokenService(UnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        public async Task CreatePaymentTokens()
        {
            decimal montoCuotaActualizado = await _UnitOfWork._CuotaRepository.ObtenerValorCuota();
            var now = DateTime.Now;
            var anioActual = now.Year;
            var semestreActual = now.Month <= 6 ? 1 : 2; ;
            int pageNumber = 1;
            int pageSize = 50;

            while (true)
            {
                var socios = await _UnitOfWork._SocioRepository.GetSociosDeudoresWithPreferenceLinkDePagoPaginado(anioActual, semestreActual, pageNumber, pageSize);

                if (socios.Count == 0)
                    break;

                var tokens = socios.Select(socio => new PaymentToken
                {
                    Id = Guid.NewGuid(),
                    IdSocio = socio.Id,
                    nombreSocio = socio.Nombre,
                    anio = anioActual,
                    semestre = semestreActual,
                    FechaExpiracion = DateOnly
                        .FromDateTime(DateTime.UtcNow)
                        .AddDays(30),
                    monto = montoCuotaActualizado,
                    usado = false
                }).ToList();

                await _UnitOfWork._PaymentTokenRepository.CreateTokens(tokens);

                pageNumber++;
            }
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

            if(!token.usado && !string.IsNullOrEmpty(token.PaymentStatus))
            {
                token.StatusDetail = null;
                token.PaymentStatus = null;
                await _UnitOfWork.SaveChangesAsync();
            }

            if (token.usado) return  Result<PaymentToken>.Error("el token ya fue utilizado", 422);
            if (hoy > token.FechaExpiracion) return Result<PaymentToken>.Error("el plazo para pagar la cuota ya finalizo", 492);

            var cuotasSocio = await _UnitOfWork._SocioRepository.GetCuotasSocioById(token.IdSocio);

            if (cuotasSocio.Any(c => c.Anio == token.anio && c.Semestre == token.semestre)) 
                return Result<PaymentToken>.Error("la cuota que se esta intenando pagar ya fue abonada", 422);

            return Result<PaymentToken>.Exito(token);
        }
    }
}

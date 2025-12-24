using APIClub.Common;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.PaymentsOnline;
using APIClub.Dtos.Payment;

namespace APIClub.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentTokenService _paymentTokenService;
        private readonly IMercadoPagoService _mercadoPagoService;
        public PaymentService (IPaymentTokenService paymentTokenService, IMercadoPagoService mercadoPagoService)
        {
            _paymentTokenService = paymentTokenService;
            _mercadoPagoService = mercadoPagoService;
        }

        public async Task<Result<PortalPaymentView>> InitPaymentProcess(Guid tokenId)
        {
            try
            {
                var result = await _paymentTokenService.ValidateToken(tokenId);

                if (!result.Exit)
                    return Result<PortalPaymentView>.Error(result.Errormessage, result.Errorcode);

                var token = result.Data;

                string semestre = token.semestre == 1 ? "primer semestre" : "segundo semestre";

                var preference_id = await _mercadoPagoService.CreatePaymentPreference($"cuota socio club de abuelos, correspondiente al {semestre} del año {token.anio}", token.monto);


                return Result<PortalPaymentView>.Exito(new PortalPaymentView
                    {
                        nombreSocio = token.nombreSocio,
                        anioPago = token.anio.ToString(),
                        semestrePago = token.semestre == 1 ? "primer semestre" : "segundo semestre",
                        monto = token.monto,
                        externalReference = token.Id,
                        Preference_Id = preference_id,
                    });
            }
            catch (Exception)
            {
                return Result<PortalPaymentView>.Error("Error al iniciar el pago", 500);    
            }
            
        }

        public async Task<Result<ProcessPaymentResponseDto>> ProcessPayment(ProcessPaymentRequestDto request)
        {
            return await _mercadoPagoService.ProcessPayment(request);
        }
    }
}

using APIClub.Application.Common;
using APIClub.Application.Dtos.Payment;
using APIClub.Application.Helpers;
using APIClub.Domain.ModuloGestionCuotas.UseCases;
using APIClub.Domain.PaymentsOnline.ExternalServices;
using APIClub.Domain.PaymentsOnline.useCases;

namespace APIClub.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentTokenService _paymentTokenService;
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly ICuotasService _cuotasService;
        private readonly UnitOfWork _unitOfWork;

        public PaymentService (IPaymentTokenService paymentTokenService, IMercadoPagoService mercadoPagoService,
            ICuotasService cuotasService ,UnitOfWork unitOfWork)
        {
            _paymentTokenService = paymentTokenService;
            _mercadoPagoService = mercadoPagoService;
            _cuotasService = cuotasService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<InfoComprobanteDto?>> getComprobante(Guid tokenId)
        {
            var token = await _unitOfWork._PaymentTokenRepository.GetToken(tokenId);

            if (token.PaymentStatus == "rejected") 
                return Result<InfoComprobanteDto?>.Error(PaymentRejectedMessageHelper.GetMessage(token.StatusDetail), 500);
            

            if(token.PaymentStatus == "approved")
            {
                var infoSocio = await _unitOfWork._SocioRepository.GetSocioById(token.IdSocio);

                var comporbante = new InfoComprobanteDto
                {
                    nombreSocio = infoSocio.Nombre + infoSocio.Apellido,
                    dniSocio = infoSocio.Dni,
                    anioPago = token.anio.ToString(),
                    semestrePagoText = PaymentDescriptionHelper.GetSemestreText(token.semestre),
                    monto = token.monto.ToString(),
                };

                return Result<InfoComprobanteDto?>.Exito(comporbante);
            }

            return Result<InfoComprobanteDto?>.Exito(null);
        }

        public async Task<Result<PortalPaymentViewDto>> InitPaymentProcess(Guid tokenId)
        {
            try
            {
                var result = await _paymentTokenService.ValidateToken(tokenId);

                if (!result.Exit)
                {
                    // Si el token ya fue usado (422), verificamos si podemos devolver el comprobante
                    if (result.Errorcode == 422)
                    {
                        var paidToken = await _unitOfWork._PaymentTokenRepository.GetToken(tokenId);
                        if (paidToken != null && (paidToken.usado || paidToken.PaymentStatus == "approved"))
                        {
                            var infoSocio = await _unitOfWork._SocioRepository.GetSocioById(paidToken.IdSocio);
                            
                            var comprobante = new InfoComprobanteDto
                            {
                                nombreSocio = infoSocio.Nombre + " " + infoSocio.Apellido, // Aseguramos nombre completo
                                dniSocio = infoSocio.Dni,
                                anioPago = paidToken.anio.ToString(),
                                semestrePagoText = PaymentDescriptionHelper.GetSemestreText(paidToken.semestre),
                                monto = paidToken.monto.ToString(),
                            };

                            return Result<PortalPaymentViewDto>.Exito(new PortalPaymentViewDto
                            {
                                AlreadyPaid = true,
                                Comprobante = comprobante
                            });
                        }
                    }

                    return Result<PortalPaymentViewDto>.Error(result.Errormessage, result.Errorcode);
                }

                var token = result.Data;
                var Preference_id = token.Preference_Id;
                string semestre = PaymentDescriptionHelper.GetSemestreText(token.semestre);

                if (string.IsNullOrEmpty(token.Preference_Id))
                {
                    string description = PaymentDescriptionHelper.BuildCuotaDescription(token.semestre, token.anio);

                    var preference_id = await _mercadoPagoService.CreatePaymentPreference(description, token.monto, token.Id.ToString());

                    token.Preference_Id = preference_id;
                    Preference_id = preference_id;

                    await _unitOfWork.SaveChangesAsync();
                }

                return Result<PortalPaymentViewDto>.Exito(new PortalPaymentViewDto
                {
                    nombreSocio = token.nombreSocio,
                    anioPago = token.anio.ToString(),
                    semestrePago = PaymentDescriptionHelper.GetSemestreText(token.semestre),
                    monto = token.monto,
                    externalReference = token.Id,
                    Preference_Id = Preference_id!,
                });

            }
            catch (Exception)
            {
                return Result<PortalPaymentViewDto>.Error("Error al iniciar el pago", 500);
            }
        }

        //no hace falta devovle rnada as que un result (MODIFICAR).
        public async Task<Result<object?>> ProcessPayment(ProcessPaymentRequestDto request)
        {
            var token = await _unitOfWork._PaymentTokenRepository.GetToken(request.externalReference);
            
            if (token == null) return Result<object?>.Error("no existe una referencia al pago que se quiere realizar dentro de nuestro sistema", 400);

            string description = PaymentDescriptionHelper.BuildCuotaDescriptionShort(token.semestre, token.anio);

            var result = await _mercadoPagoService.ProcessPayment(request, token.monto, description);

            if (!result.Exit) return Result<object?>.Error(result.Errormessage, result.Errorcode);

            return Result<object?>.Exito(null);
        }

        public async Task RegistrarPago(MercadoPagoWebhookDto notification)
        {
            try
            {
                if (notification.Type == "payment" && notification.Data?.Id != null)
                {
                    string paymentId = notification.Data.Id;
                    var paymentInfo = await _mercadoPagoService.GetPayment(paymentId);

                    Console.WriteLine($"Pago recibido: ID {paymentId}, Status {paymentInfo.Status}");

                    if (!Guid.TryParse(paymentInfo.ExternalReference, out Guid tokenId))
                    {
                        Console.WriteLine($"ExternalReference inválido: {paymentInfo.ExternalReference}");
                        return;
                    }

                    var token = await _unitOfWork._PaymentTokenRepository.GetToken(tokenId);
                    
                    if (token == null)
                    {
                        Console.WriteLine($"Token no encontrado: {tokenId}");
                        return;
                    }

                    // Procesar pago aprobado
                    if (paymentInfo.Status == "approved")
                    {
                        // Intento atómico de confirmar el token
                        var confirmado = await _unitOfWork
                            ._PaymentTokenRepository
                            .confirmPaymentIfNotWereUsed(token.Id);

                        if (!confirmado)
                        {
                            Console.WriteLine($"Webhook duplicado ignorado. Token {token.Id} ya confirmado.");
                            return;
                        }

                        var result = await _cuotasService.RegistrarPagoCuoataOnline(token);

                        if (!result.Exit)
                        {
                            Console.WriteLine($"Error registrando cuota para token {token.Id}");
                            return;
                        }

                        Console.WriteLine("Pago de cuota registrado exitosamente");
                        return;
                    }
                    else if(paymentInfo.Status == "rejected")
                    {
                        Console.WriteLine($"Pago de cuota rechazado: ({paymentInfo.statusDetail})");
                        token.PaymentStatus = paymentInfo.Status;
                        token.StatusDetail = paymentInfo.statusDetail;
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando webhook: {ex.Message}");
            }
        }
    }
}

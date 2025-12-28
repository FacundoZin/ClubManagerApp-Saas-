using APIClub.Common;
using APIClub.Domain.GestionSocios;
using APIClub.Domain.PaymentsOnline;
using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Dtos.Payment;
using APIClub.Helpers;

namespace APIClub.Services
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

            if (token.PaymentStatus == PaymentStatus.Rejected) return Result<InfoComprobanteDto?>.Error("el pago fallo al procesarse, porfavor intentelo mas tarde", 500);
            if (token.PaymentStatus == PaymentStatus.Pending) return Result<InfoComprobanteDto?>.Exito(null);

            if(token.PaymentStatus == PaymentStatus.ApprovedByGateway)
            {
                var infoSocio = await _unitOfWork._SocioRepository.GetSocioById(token.IdSocio);

                var comporbante = new InfoComprobanteDto
                {
                    nombreSocio = infoSocio.Nombre,
                    dniSocio = infoSocio.Dni,
                    anioPago = token.anio.ToString(),
                    semestrePagoText = PaymentDescriptionHelper.GetSemestreText(token.semestre),
                    monto = token.monto.ToString(),
                };

                return Result<InfoComprobanteDto?>.Exito(comporbante);
            }

            return Result<InfoComprobanteDto?>.Error("el pago fallo al procesarse, porfavor intentelo mas tarde", 500);
        }

        public async Task<Result<PortalPaymentViewDto>> InitPaymentProcess(Guid tokenId)
        {
            try
            {
                var result = await _paymentTokenService.ValidateToken(tokenId);

                if (!result.Exit)
                    return Result<PortalPaymentViewDto>.Error(result.Errormessage, result.Errorcode);

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

        public async Task<Result<ProcessPaymentResponseDto>> ProcessPayment(ProcessPaymentRequestDto request)
        {
            var token = await _unitOfWork._PaymentTokenRepository.GetToken(request.externalReference);
            
            if (token == null) return Result<ProcessPaymentResponseDto>.Error("no existe una referencia al pago que se quiere realizar dentro de nuestro sistema", 400);

            string description = PaymentDescriptionHelper.BuildCuotaDescriptionShort(token.semestre, token.anio);

            var result = await _mercadoPagoService.ProcessPayment(request, token.monto, description);

            if (!result.Exit)
            {
                token.PaymentStatus = PaymentStatus.Rejected;
                await _unitOfWork.SaveChangesAsync();
            }

            // Usar mapper centralizado para estados de Mercado Pago
            token.PaymentStatus = PaymentStatusMapper.MapFromMercadoPago(result.Data.status);

            await _unitOfWork.SaveChangesAsync();
            return result;
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

                    // VALIDACIÓN DE IDEMPOTENCIA - Previene procesamiento duplicado de webhooks
                    if (token.usado)
                    {
                        Console.WriteLine($"Webhook duplicado detectado. Token {tokenId} ya fue procesado.");
                        return;
                    }

                    if(paymentInfo.Status == "rejected" || paymentInfo.Status == "cancelled")
                    {
                        token.PaymentStatus = PaymentStatus.Rejected;
                        await _unitOfWork.SaveChangesAsync();
                        Console.WriteLine($"Pago rechazado/cancelado. Token {tokenId} actualizado.");
                    }

                    // Procesar pago aprobado
                    if (PaymentStatusMapper.IsSuccessStatus(paymentInfo.Status))
                    {
                        var result = await _cuotasService.RegistrarPagoCuoataOnline(token);

                        if (result.Exit)
                        {
                            Console.WriteLine("Pago de cuota registrado exitosamente");
                            token.usado = true;
                            await _unitOfWork.SaveChangesAsync();
                        }
                        
                        else
                            Console.WriteLine($"Error registrando pago de cuota: {result.Errormessage}");
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

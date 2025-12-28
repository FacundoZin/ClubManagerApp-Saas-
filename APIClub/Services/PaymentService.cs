using APIClub.Common;
using APIClub.Domain.GestionSocios;
using APIClub.Domain.PaymentsOnline;
using APIClub.Dtos.Payment;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

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

        public async Task<Result<PortalPaymentView>> InitPaymentProcess(Guid tokenId)
        {
            try
            {
                var result = await _paymentTokenService.ValidateToken(tokenId);

                if (!result.Exit)
                    return Result<PortalPaymentView>.Error(result.Errormessage, result.Errorcode);

                var token = result.Data;
                var Preference_id = token.Preference_Id;
                string semestre = token.semestre == 1 ? "primer semestre" : "segundo semestre";

                if (string.IsNullOrEmpty(token.Preference_Id))
                {
                    string description = $"cuota socio club de abuelos, correspondiente al {semestre} del año {token.anio}";

                    var preference_id = await _mercadoPagoService.CreatePaymentPreference(description, token.monto, token.Id.ToString());

                    token.Preference_Id = preference_id;
                    Preference_id = preference_id;

                    await _unitOfWork.SaveChangesAsync();
                }

                return Result<PortalPaymentView>.Exito(new PortalPaymentView
                {
                    nombreSocio = token.nombreSocio,
                    anioPago = token.anio.ToString(),
                    semestrePago = token.semestre == 1 ? "primer semestre" : "segundo semestre",
                    monto = token.monto,
                    externalReference = token.Id,
                    Preference_Id = Preference_id!,
                });

            }
            catch (Exception)
            {
                return Result<PortalPaymentView>.Error("Error al iniciar el pago", 500);
            }

        }

        public async Task<Result<ProcessPaymentResponseDto>> ProcessPayment(ProcessPaymentRequestDto request)
        {
            var token = await _unitOfWork._PaymentTokenRepository.GetToken(request.externalReference);
            
            if (token == null) return Result<ProcessPaymentResponseDto>.Error("no existe una referencia al pago que se quiere realizar dentro de nuestro sistema", 400);
            string semestre = token.semestre == 1 ? "primer semestre" : "segundo semestre";

            string description = $"cuota club abuelos correspondiente al {semestre} del año {token.anio}";

            return await _mercadoPagoService.ProcessPayment(request, token.monto, description);
        }

        public async Task RegistrarPago(JsonElement notification)
        {
            try
            {
                string? type = null;
                string? paymentId = null;

                if (notification.TryGetProperty("type", out var typeProp))
                {
                    type = typeProp.GetString();
                }

                if (type == "payment")
                {
                    if (notification.TryGetProperty("data", out var data) && data.TryGetProperty("id", out var idProp))
                    {
                        paymentId = idProp.ValueKind == JsonValueKind.Number ? idProp.GetInt64().ToString() : idProp.GetString();
                    }
                }

                if (!string.IsNullOrEmpty(paymentId))
                {
                    var paymentInfo = await _mercadoPagoService.GetPayment(paymentId);

                    Console.WriteLine($"Pago recibido: ID {paymentId}, Status {paymentInfo.Status}");

                    if (paymentInfo.Status == "approved")
                    {


                        string externalReference = paymentInfo.ExternalReference;

                        var token = await _unitOfWork._PaymentTokenRepository.GetToken(Guid.Parse(externalReference));


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

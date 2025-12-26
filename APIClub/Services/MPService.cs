using APIClub.Common;
using APIClub.Domain.PaymentsOnline;
using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Dtos.Payment;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;

namespace APIClub.Services
{
    public class MPService : IMercadoPagoService
    {

        private readonly PreferenceClient _client;
        private readonly PaymentClient _paymentClient;

        public MPService(IConfiguration configuration, UnitOfWork unitOfWork)
        {
            MercadoPagoConfig.AccessToken = configuration["MercadoPago:AccessToken"];
            _client = new PreferenceClient();
            _paymentClient = new PaymentClient();
        }

        public async Task<string> CreatePaymentPreference(string title, decimal montoCuota, string externalReference)
        {
            try
            {
                var request = new PreferenceRequest
                {
                    ExternalReference = externalReference,
                    PaymentMethods = new PreferencePaymentMethodsRequest
                    {
                        ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>{
                            new PreferencePaymentTypeRequest { Id = "ticket" }
                        }
                    },
                    Items = new List<PreferenceItemRequest>
                    {
                        new PreferenceItemRequest
                        {
                            Title = title,
                            Quantity = 1,
                            CurrencyId = "ARS",
                            UnitPrice = montoCuota,
                        },
                    },
                };

                Preference preference = await _client.CreateAsync(request);
                return preference.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear la preferencia de pago: {ex.Message}", ex);
            }
        }

        public Task<mpPaymentInfo> GetPayment(string paymentId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<ProcessPaymentResponseDto>> ProcessPayment(ProcessPaymentRequestDto request, decimal amount, string description)
        {
            try
            {
                // Crear el request de pago para MercadoPago
                var paymentRequest = new PaymentCreateRequest
                {
                    Token = request.token,
                    IssuerId = request.issuer_id,
                    PaymentMethodId = request.payment_method_id,
                    TransactionAmount = amount,
                    Installments = request.installments,
                    Payer = new PaymentPayerRequest
                    {
                        Email = request.payer.email,
                        Identification = new IdentificationRequest
                        {
                            Type = request.payer.identification.type,
                            Number = request.payer.identification.number
                        }
                    },
                    ExternalReference = request.externalReference.ToString(),
                    Description = description,
                };

                // Procesar el pago con MercadoPago
                Payment payment = await _paymentClient.CreateAsync(paymentRequest);

                // Verificar el resultado
                if (payment == null)
                {
                    return Result<ProcessPaymentResponseDto>.Error(
                        "No se recibió respuesta de MercadoPago",
                        500
                    );
                }

                // Crear el DTO de respuesta
                var responseDto = new ProcessPaymentResponseDto
                {
                    paymentId = payment.Id,
                    status = payment.Status,
                    status_detail = payment.StatusDetail
                };

                // Verificar si el pago fue aprobado
                if (payment.Status == "approved")
                {
                    return Result<ProcessPaymentResponseDto>.Exito(responseDto);
                }
                else if (payment.Status == "rejected")
                {
                    return Result<ProcessPaymentResponseDto>.Error(
                        $"Pago rechazado: {payment.StatusDetail}",
                        400
                    );
                }
                else
                {
                    // Estados como "pending", "in_process", etc.
                    return Result<ProcessPaymentResponseDto>.Exito(responseDto);
                }
            }
            catch (Exception ex)
            {
                return Result<ProcessPaymentResponseDto>.Error(
                    $"Error al procesar el pago: {ex.Message}",
                    500
                );
            }
        }
    }

}

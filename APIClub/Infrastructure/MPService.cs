using APIClub.Application.Common;
using APIClub.Application.Dtos.Payment;
using APIClub.Domain.PaymentsOnline.ExternalServices;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;

namespace APIClub.Infrastructure
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
                        ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                        {
                            new PreferencePaymentTypeRequest { Id = "credit_card" },
                            new PreferencePaymentTypeRequest { Id = "debit_card" },
                            new PreferencePaymentTypeRequest { Id = "ticket" },
                            new PreferencePaymentTypeRequest { Id = "bank_transfer" }
                        },
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

        public async Task<mpPaymentInfoDto> GetPayment(string paymentId)
        {
            try
            {
                if (!long.TryParse(paymentId, out long id))
                {
                    throw new ArgumentException("Id de pago inválido", nameof(paymentId));
                }

                Payment payment = await _paymentClient.GetAsync(id);

                if (payment == null)
                    throw new Exception("No se encontró el pago en Mercado Pago");

                return new mpPaymentInfoDto
                {
                    PaymentId = payment.Id.ToString()!,
                    Status = payment.Status,
                    statusDetail = payment.StatusDetail,
                    ExternalReference = payment.ExternalReference,
                    Amount = payment.TransactionAmount ?? 0,
                    PaymentMethod = payment.PaymentMethodId,
                    ApprovedAt = payment.DateApproved
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el pago de Mercado Pago: {ex.Message}", ex);
            }
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

                return Result<ProcessPaymentResponseDto>.Exito(responseDto);
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

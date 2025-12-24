using APIClub.Common;
using APIClub.Domain.PaymentsOnline;
using APIClub.Dtos.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;

namespace APIClub.Services
{
    public class MPService : IMercadoPagoService
    {

        private readonly PreferenceClient _client;

        public MPService(IConfiguration configuration)
        {
            MercadoPagoConfig.AccessToken = configuration["MercadoPago:AccessToken"];
            _client = new PreferenceClient();
        }

        public async Task<string> CreatePaymentPreference(string title, decimal montoCuota)
        {
            try
            {
                var request = new PreferenceRequest
                {
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

    }

}

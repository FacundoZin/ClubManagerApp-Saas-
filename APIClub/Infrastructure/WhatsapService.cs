using APIClub.Application.Common;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.Notificaciones.Infra;
using APIClub.Domain.Notificaciones.Models;
using Microsoft.Extensions.Options;
using System.Net;

namespace APIClub.Infrastructure
{
    public class WhatsapService : IWhatsappService
    {
        private readonly ISocioRepository _SocioRepository;
        private readonly WhatsAppConfig _whatsAppConfig;
        private readonly HttpClient _httpClient;

        public WhatsapService(
            ISocioRepository socioRepository,
            IOptions<WhatsAppConfig> whatsAppConfig,
            HttpClient httpClient)
        {
            _SocioRepository = socioRepository;
            _whatsAppConfig = whatsAppConfig.Value;
            _httpClient = httpClient;
        }

        public async Task<Result<object?>> SendNotificacionPagoCuota()
        {
            try
            {
                var hoy = DateTime.Now;
                var anio = hoy.Year;
                var semestre = hoy.Month > 6 ? 2 : 1;

                var socios = await _SocioRepository.GetSociosDeudores(anio, semestre);
                var sociosConTelefono = socios.Where(s => !string.IsNullOrEmpty(s.Telefono)).ToList();

                if (!sociosConTelefono.Any())
                {
                    Console.WriteLine("[INFO] No hay socios deudores con teléfono.");
                    return Result<object?>.Exito(null);
                }

                Console.WriteLine(
                    $"[INFO] Iniciando envío de notificaciones. Total: {sociosConTelefono.Count}");

                int exitosos = 0;
                int fallidos = 0;

                int maxRetries = 3;

                foreach (var socio in sociosConTelefono)
                {
                    var semestreTexto = semestre == 1 ? "primer semestre" : "segundo semestre";
                    var intento = 0;
                    var enviado = false;

                    while (!enviado && intento <= maxRetries)
                    {
                        try
                        {
                            intento++;

                            var response = await SendWhatsAppPaymentMessage(
                                socio.Telefono!,
                                socio.Nombre,
                                semestreTexto,
                                anio.ToString());

                            if (response.IsSuccessStatusCode)
                            {
                                exitosos++;
                                enviado = true;
                            }
                            else if (EsErrorRecuperable(response))
                            {
                                Console.WriteLine(
                                    $"[WARN] Error temporal ({response.StatusCode}) para {socio.Nombre}. Retry {intento}/{maxRetries}");
                                await Task.Delay(1500);
                            }
                            else
                            {
                                Console.WriteLine(
                                    $"[ERROR] Error definitivo ({response.StatusCode}) para {socio.Nombre}. No retry.");
                                fallidos++;
                                break;
                            }
                        }
                        catch (Exception ex) when (EsExcepcionRecuperable(ex))
                        {
                            Console.WriteLine(
                                $"[WARN] Error de red para {socio.Nombre}. Retry {intento}/{maxRetries}: {ex.Message}");
                            await Task.Delay(1500);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"[ERROR] Error inesperado para {socio.Nombre}: {ex.Message}");
                            fallidos++;
                            break;
                        }
                    }
                }

                Console.WriteLine(
                    $"[INFO] Proceso finalizado. Exitosos: {exitosos}, Fallidos: {fallidos}");

                if (fallidos > 0)
                    return Result<object?>.Error("Hubo mensajes que no fueron enviados", 500);

                return Result<object?>.Exito(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FATAL] Error general en notificaciones: {ex}");
                return Result<object?>.Error("Error general en el proceso de notificaciones", 500);
            }
        }

        public async Task<Result<object?>> SendWhatsAppTest(string telefono, string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(telefono))
                    return Result<object?>.Error("El teléfono es obligatorio", 400);

                var hoy = DateTime.Now;
                var anio = hoy.Year.ToString();
                var semestreTexto = hoy.Month > 6 ? "segundo semestre" : "primer semestre";

                var response = await SendWhatsAppPaymentMessage(
                    telefono,
                    nombre,
                    semestreTexto,
                    anio
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"[ERROR] WhatsApp API error: {errorBody}");

                    return Result<object?>.Error(
                        $"WhatsApp API error: {errorBody}",
                        (int)response.StatusCode
                    );
                }

                Console.WriteLine("[INFO] WhatsApp test enviado correctamente");
                return Result<object?>.Exito(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error en WhatsApp test: {ex}");
                return Result<object?>.Error("Error interno en envío de prueba", 500);
            }
        }


        public async Task<HttpResponseMessage> SendWhatsAppPaymentMessage(string telefono, string nombreSocio, string semestreTexto, string anio)
        {

            var requestUrl =
                $"/{_whatsAppConfig.ApiVersion}/{_whatsAppConfig.PhoneNumberId}/messages";

            var messageRequest = new WhatsAppMessageRequest
            {
                To = telefono,
                Type = "template",
                Template = new WhatsAppTemplate
                {
                    Name = "notificacion_pago_cuota",
                    Language = new WhatsAppLanguage { Code = "es_AR" },
                    Components = new List<WhatsAppComponent>
                    {
                        new WhatsAppComponent
                        {
                            Type = "body",
                            Parameters = new List<WhatsAppParameter>
                            {
                                new() { Text = nombreSocio },
                                new() { Text = semestreTexto },
                                new() { Text = anio }
                            }
                        }
                    }
                }
            };


            var response = await _httpClient.PostAsJsonAsync(requestUrl, messageRequest);

            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("========== WHATSAPP RESPONSE ==========");
            Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine(responseBody);
            Console.WriteLine("======================================");

            return response;
        }


        private static bool EsErrorRecuperable(HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.TooManyRequests ||
                   (int)response.StatusCode >= 500;
        }

        private static bool EsExcepcionRecuperable(Exception ex)
        {
            return ex is HttpRequestException ||
                   ex is TaskCanceledException;
        }
    }
}

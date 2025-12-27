using APIClub.Domain.PaymentsOnline;
using APIClub.Dtos.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Inicia el proceso de pago usando un token
        /// </summary>
        /// <param name="PaymentToken">Id del token de pago</param>
        /// <returns>Información de la preferencia de pago</returns>
        [HttpPost("initPayment")]
        public async Task<IActionResult> InitPaymentProcess()
        {
            try
            {

                if (!Request.Headers.TryGetValue("PaymentToken", out var tokenHeader))
                    return BadRequest(new { message = "Token no proporcionado" });

                if (!Guid.TryParse(tokenHeader, out Guid tokenId))
                    return BadRequest(new { message = "Token inválido" });

                var result = await _paymentService.InitPaymentProcess(tokenId);

                if (!result.Exit)
                    return StatusCode(result.Errorcode, new { exit = false, errormessage = result.Errormessage });

                return Ok(new { exit = true, data = result.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al procesar el pago", detail = ex.Message });
            }
        }

        /// <summary>
        /// Procesa el pago recibido desde el Payment Brick
        /// </summary>
        /// <param name="request">Datos del pago desde el frontend</param>
        /// <returns>Resultado del procesamiento del pago</returns>
        [HttpPost("processPayment")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequestDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { exit = false, errormessage = "Datos de pago no proporcionados" });

                if (string.IsNullOrEmpty(request.token))
                    return BadRequest(new { exit = false, errormessage = "Token de pago requerido" });

                if (request.externalReference == Guid.Empty)
                    return BadRequest(new { exit = false, errormessage = "Referencia externa requerida" });

                var result = await _paymentService.ProcessPayment(request);

                if (!result.Exit)
                    return StatusCode(result.Errorcode, result.Errormessage);

                return Ok(new { exit = true, data = result.Data });
            }
            catch (Exception ex)
            {
                return Ok(new { exit = false, errormessage = $"Error al procesar el pago: {ex.Message}" });
            }
        }

        /// <summary>
        /// Endpoint para recibir notificaciones de Webhooks de Mercado Pago
        /// </summary>
        /// <param name="notification">Cuerpo de la notificación en formato JSON</param>
        /// <returns>Estado de recepción</returns>
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement notification)
        {
            try
            {
                // Registramos la notificación para debug (opcional)
                // Console.WriteLine($"Webhook recibido: {notification.ToString()}");

                string? type = null;
                string? paymentId = null;

                // Intentamos obtener el tipo de notificación
                if (notification.TryGetProperty("type", out var typeProp))
                {
                    type = typeProp.GetString();
                }

                // Si es un pago, el ID suele venir en data.id
                if (type == "payment")
                {
                    if (notification.TryGetProperty("data", out var data) && data.TryGetProperty("id", out var idProp))
                    {
                        paymentId = idProp.ValueKind == JsonValueKind.Number ? idProp.GetInt64().ToString() : idProp.GetString();
                    }
                }
                // Soporte para formato antiguo (topic=payment e id en la raíz)
                else if (notification.TryGetProperty("topic", out var topicProp) && topicProp.GetString() == "payment")
                {
                    if (notification.TryGetProperty("id", out var idProp))
                    {
                        // El id puede venir como string o como número
                        paymentId = idProp.ValueKind == JsonValueKind.Number ? idProp.GetInt64().ToString() : idProp.GetString();
                    }
                }

                if (!string.IsNullOrEmpty(paymentId))
                {
                    // Obtenemos info detallada del pago desde Mercado Pago
                    var paymentInfo = await _paymentService.GetPayment(paymentId);

                    // Extraemos la external_reference que guardamos al crear la preferencia
                    string externalReference = paymentInfo.ExternalReference;

                    // IMPORTANTE: Aquí debes implementar tu lógica de registro en el sistema
                    // Por ejemplo: await _cuotaService.ConfirmarPago(externalReference, paymentInfo);
                    
                    Console.WriteLine($"Pago recibido: ID {paymentId}, Status {paymentInfo.Status}, Ref {externalReference}");
                }

                // Mercado Pago requiere una respuesta 200 (Ok) o 201 para confirmar recepción
                // Si no respondes con éxito, Mercado Pago reintentará enviar la notificación
                return Ok();
            }
            catch (Exception ex)
            {
                // En caso de error, logueamos pero devolvemos Ok para evitar reintentos infinitos si es un bug de código
                // En un entorno real, podrías querer devolver Error para reintentos si el problema es temporal (ej. DB caída)
                Console.WriteLine($"Error procesando webhook: {ex.Message}");
                return Ok();
            }
        }
    }
}

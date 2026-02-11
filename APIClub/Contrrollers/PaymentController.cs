using APIClub.Application.Dtos.Payment;
using APIClub.Domain.GestionSocios;
using APIClub.Domain.PaymentsOnline.useCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


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
        public async Task<IActionResult> Webhook([FromBody] MercadoPagoWebhookDto notification)
        {
            await _paymentService.RegistrarPago(notification);
            return Ok();
        }

        /// <summary>
        /// Obtiene la información del comprobante si el pago fue aprobado
        /// </summary>
        /// <returns>Información del comprobante o null si aún está pendiente</returns>
        [HttpGet("comprobante")]
        public async Task<IActionResult> GetComprobante()
        {
            try
            {
                if (!Request.Headers.TryGetValue("PaymentToken", out var tokenHeader))
                    return BadRequest(new { message = "Token no proporcionado" });

                if (!Guid.TryParse(tokenHeader, out Guid tokenId))
                    return BadRequest(new { message = "Token inválido" });

                var result = await _paymentService.getComprobante(tokenId);

                if (!result.Exit)
                    return StatusCode(result.Errorcode, new { exit = false, errormessage = result.Errormessage });

                return Ok(new { exit = true, data = result.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el comprobante", detail = ex.Message });
            }
        }
    }
}

using APIClub.Domain.PaymentsOnline;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MercadoPagoController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public MercadoPagoController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Inicia el proceso de pago usando un token
        /// </summary>
        /// <param name="tokenId">Id del token de pago</param>
        /// <returns>Información de la preferencia de pago</returns>
        [HttpPost("initPayment")]
        public async Task<IActionResult> InitPaymentProcess()
        {
            try
            {

                if (!Request.Headers.TryGetValue("X-Token-Id", out var tokenHeader))
                    return BadRequest(new { message = "Token no proporcionado" });

                if (!Guid.TryParse(tokenHeader, out Guid tokenId))
                    return BadRequest(new { message = "Token inválido" });

                var result = await _paymentService.InitPaymentProcess(tokenId);

                if (!result.Exit)
                    return StatusCode(result.Errorcode, result.Errormessage);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al procesar el pago", detail = ex.Message });
            }
        }
    }
}

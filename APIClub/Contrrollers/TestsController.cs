using APIClub.Application.Dtos.Whatsapp;
using APIClub.Domain.Notificaciones.Infra;
using APIClub.Domain.Notificaciones.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly IWhatsappService _notifyService;
        private readonly WhatsAppConfig _whatsAppConfig;

        public TestsController(IWhatsappService notifyService, IOptions<WhatsAppConfig> whatsAppConfig)
        {
            _notifyService = notifyService;
            _whatsAppConfig = whatsAppConfig.Value;
        }

        [HttpPost("sandbox/whatsapp/test")]
        public async Task<IActionResult> TestWhatsApp([FromBody] WhatsappTestRequest request)
        {
            var result = await _notifyService.SendWhatsAppTest(
                request.Telefono,
                request.nombreSocio
            );

            return result.Exit
                ? Ok("Mensaje enviado")
                : StatusCode(result.Errorcode, result.Errormessage);
        }

        [HttpGet("whatsapp/webhook")]
        public IActionResult VerifyWebhook(
            [FromQuery(Name = "hub.mode")] string? mode,
            [FromQuery(Name = "hub.challenge")] string? challenge,
            [FromQuery(Name = "hub.verify_token")] string? verifyToken)
        {
            Console.WriteLine("WhatsApp Webhook Verification attempt...");
            
            if (mode == "subscribe" && verifyToken == _whatsAppConfig.VerifyToken)
            {
                Console.WriteLine("Verification successful!");
                return Content(challenge ?? "", "text/plain");
            }

            Console.WriteLine("Verification failed.");
            return StatusCode(403);
        }

        [HttpPost("whatsapp/webhook")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] JsonElement payload)
        {
            Console.WriteLine("========== WHATSAPP WEBHOOK RECEIVED ==========");

            Console.WriteLine("Method:");
            Console.WriteLine(Request.Method);

            Console.WriteLine("Headers:");
            foreach (var header in Request.Headers)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }

            Console.WriteLine("Body (raw):");

            Request.EnableBuffering();

            using var reader = new StreamReader(Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            Request.Body.Position = 0;

            Console.WriteLine(body);

            Console.WriteLine("===============================================");

            return Ok();
        }
    }
}

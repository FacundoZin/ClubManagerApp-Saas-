using APIClub.Application.Dtos.Whatsapp;
using APIClub.Domain.Background;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly INotifyService _notifyService;

        public TestsController(INotifyService notifyService)
        {
            _notifyService = notifyService;
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
                : StatusCode(500, result.Errormessage);
        }

    }
}

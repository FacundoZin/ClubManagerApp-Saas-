using APIClub.Application.Dtos.Viajes.FileViaje;
using APIClub.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIClub.Controllers
{
    [ApiController]
    [Route("api/file-viaje")]
    public class FileViajeController : ControllerBase
    {
        private readonly FileViajeService _service;

        public FileViajeController(FileViajeService service)
        {
            _service = service;
        }

        [HttpPost("add-inscripto")]
        public async Task<IActionResult> AddInscripto([FromBody] AddInscriptoToFileDto dto)
        {
            var success = await _service.AddInscriptoToFile(dto);

            if (!success)
                return BadRequest("Inscripto no encontrado o número de file inválido");

            return Ok();
        }

        [HttpGet("viaje/{viajeId}")]
        public async Task<IActionResult> GetByViaje(int viajeId)
        {
            var files = await _service.GetFilesByViaje(viajeId);
            return Ok(files);
        }
    }
}

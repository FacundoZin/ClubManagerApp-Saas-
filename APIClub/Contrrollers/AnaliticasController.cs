using APIClub.Domain.Analiticas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnaliticasController : ControllerBase
    {
        private readonly IAnaliticasService _service;

        public AnaliticasController(IAnaliticasService service)
        {
            _service = service;
        }

        [HttpGet("resumen-socios")]
        public async Task<IActionResult> GetSocios([FromQuery] int? anio, [FromQuery] int? mes, [FromQuery] int? semestre)
        {
            int year = anio ?? DateTime.Now.Year;
            var result = await _service.GetResumenSocios(year, mes, semestre);

            if (!result.Exit)
                return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(result.Data);
        }

        [HttpGet("movimiento-socios")]
        public async Task<IActionResult> GetMovimientoDeSocios([FromQuery] int? anio, [FromQuery] int? semestre)
        {
            int year = anio ?? DateTime.Now.Year;
            var result = await _service.GetMovimientoDeSocios(year, semestre);

            if (!result.Exit)
                return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(result.Data);
        }

        [HttpGet("resumen-ingresos")]
        public async Task<IActionResult> GetIngresos([FromQuery] int? anio, [FromQuery] int? mes, [FromQuery] int? semestre)
        {
            int year = anio ?? DateTime.Now.Year;
            var result = await _service.GetResumenIngresos(year, mes, semestre);

            if (!result.Exit)
                return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(result.Data);
        }

        [HttpGet("historial-ingresos")]
        public async Task<IActionResult> GetHistorialIngresos([FromQuery] int? anio, [FromQuery] int? semestre)
        {
            int year = anio ?? DateTime.Now.Year;
            var result = await _service.GetHistorialIngresos(year, semestre);

            if (!result.Exit)
                return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(result.Data);
        }

        [HttpGet("distribucion-pago")]
        public async Task<IActionResult> GetDistribucionPagoCuotas([FromQuery] int? anio, [FromQuery] int? semestre)
        {
            int year = anio ?? DateTime.Now.Year;
            var result = await _service.GetDistribucionFormaPago(year, semestre);

            if (!result.Exit)
                return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(result.Data);
        }
    }
}

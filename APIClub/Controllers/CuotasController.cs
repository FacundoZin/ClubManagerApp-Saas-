using APIClub.Application.Dtos.Cuota;
using APIClub.Domain.ModuloGestionCuotas.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CuotasController : ControllerBase
    {
        private readonly ICuotasService _CuotasService;
        public CuotasController(ICuotasService cuotaService)
        {
            _CuotasService = cuotaService;
        }

        [HttpPost("actualizarValor")]
        public async Task<IActionResult> ActualizarValorCuota([FromBody] decimal nuevoValor)
        {
            var result = await _CuotasService.ActualizarValorCuota(nuevoValor);
            return Ok(new { result.Data });
        }

        [HttpPost("RegistrarPagos")]
        public async Task<IActionResult> RegistrarPagos([FromBody] RegistCuotaInRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _CuotasService.RegistrarPagosCuotas(request.IdSocio, request.Periodos, request.FormaPago);

            if (!result.Exit)
                return StatusCode(result.Errorcode, new { mensaje = result.Errormessage });

            return Ok(new { data = result.Data });
        }

        [HttpPost("RegistrarPagoConCobrador")]
        public async Task<IActionResult> RegistrarPagoDeCuotaConCobrador([FromBody] RegistCuotaInRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var idCobrador = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _CuotasService.RegistrarPagosCuotasCobrador(request.IdSocio, request.Periodos, idCobrador);

            if (!result.Exit) return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(new { data = result.Data });
        }

        [HttpGet("HistorialCuotas")]
        public async Task<IActionResult> VerHistorialCuotas(
            [FromQuery] string tipoFiltro = "fecha",
            [FromQuery] string? fechaPago = null,
            [FromQuery] int? anio = null,
            [FromQuery] int? semestre = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var request = new HistorialCuotasRequestDto
            {
                TipoFiltro = tipoFiltro,
                FechaPago = !string.IsNullOrEmpty(fechaPago) ? DateOnly.Parse(fechaPago) : null,
                Anio = anio,
                Semestre = semestre,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _CuotasService.VerHistorialCuotas(request);

            if (!result.Exit)
                return StatusCode(result.Errorcode, new { mensaje = result.Errormessage });

            return Ok(new { data = result.Data });
        }
    }
}

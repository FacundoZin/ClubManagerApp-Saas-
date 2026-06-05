using APIClub.Application.Dtos.Lote;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using APIClub.Domain.ModuloGestionCobradores.UseCases;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CobranzasController : ControllerBase
    {
        private readonly ICobranzasServices _cobranzasServices;
        public CobranzasController(ICobranzasServices cobranzasServices)
        {
            _cobranzasServices = cobranzasServices;
        }

        [HttpGet("lotes/{IdLote}/deudores")]
        public async Task<IActionResult> ListarSociosDeudoresPorLote(int IdLote, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _cobranzasServices.ListarSociosDedudoresPorLote(IdLote, pageNumber, pageSize);

            if (!result.Exit) return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(result.Data);
        }

        [HttpGet("lotes")]
        public async Task<IActionResult> ListarLotes()
        {
            var lotes = await _cobranzasServices.GetLotesPreview();

            return Ok(lotes);
        }

        [HttpPost("lotes")]
        public async Task<IActionResult> CrearLote(CreateLoteDto dto)
        {
            var result = await _cobranzasServices.CrearLote(dto);

            if (!result.Exit) return BadRequest(result.Errormessage);

            return Ok(result.Data);
        }

        [HttpGet("cobrador/historial")]
        public async Task<IActionResult> VerHistorialDeCobradorByMes([FromQuery] int mes, [FromQuery] int anio, [FromQuery] int idCobrador)
        {
            var historialCobros = await _cobranzasServices.GetHistorialCobradorByMes(idCobrador, mes, anio);
            return Ok(historialCobros);
        }


        [HttpGet("cobradores")]
        public async Task<IActionResult> VerListadoDeCobradores()
        {
            var cobradores = await _cobranzasServices.GetListaCobradores();
            return Ok(cobradores);
        }

    }
}

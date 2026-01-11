using APIClub.Domain.GestionSocios;
using Microsoft.AspNetCore.Mvc;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CobranzasController : ControllerBase
    {
        private readonly ICobranzasServices _cobranzasServices;
        public CobranzasController(ICobranzasServices cobranzasServices)
        {
            _cobranzasServices = cobranzasServices;
        }

        [HttpGet("lotes/{IdLote}/deudores")]
        public async Task<IActionResult> ListarSociosDeudoresPorLote(int IdLote)
        {
            var result = await _cobranzasServices.ListarSociosDedudoresPorLote(IdLote);

            if(!result.Exit) return BadRequest(result.Errormessage);

            return Ok(result.Data);
        }

        [HttpGet("lotes")]
        public async Task<IActionResult> ListarLotes(int IdLote)
        {
            var lotes = await _cobranzasServices.GetLotesPreview();

            return Ok(lotes);   
        }

    }
}

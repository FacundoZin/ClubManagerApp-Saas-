using APIClub.Application.Dtos.Cuota;
using APIClub.Domain.GestionSocios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("pagarCuota")]
        public async Task<IActionResult> RegistrarCuota([FromBody] RegistCuotaRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _CuotasService.RegistrarPagoCuoata(request.IdSocio, request.FormaPago);

            if (!result.Exit)
                return StatusCode(result.Errorcode, new { mensaje = result.Errormessage });

            return Ok(new { data = result.Data });
        }

       
    }
}

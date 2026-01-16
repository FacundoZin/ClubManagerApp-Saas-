using APIClub.Application.Dtos.AlquilerDeArticulos;
using APIClub.Application.Dtos.ItemsAlquiler;
using APIClub.Domain.AlquilerArticulos;
using Microsoft.AspNetCore.Mvc;
using APIClub.Application.Common;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlquileresController : ControllerBase
    {
        private readonly IAlquilerArticulosService _alquileresService;

        public AlquileresController(IAlquilerArticulosService alquileresService)
        {
            _alquileresService = alquileresService;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarAlquiler([FromBody] CreateAlquilerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _alquileresService.RegistrarAlquiler(dto);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlquilerById(int id)
        {
            var result = await _alquileresService.GetAlquilerById(id);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return Ok(result.Data);
        }

        [HttpGet("activos")]
        public async Task<IActionResult> GetAlquileresActivos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 12)
        {
            var result = await _alquileresService.GetAlquileresActivos(pageNumber, pageSize);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return Ok(result.Data);
        }

        [HttpGet("socio/{socioDni}")]
        public async Task<IActionResult> GetAlquilerBySocio(string socioDni)
        {
            var result = await _alquileresService.GetAlquilerBySocio(socioDni);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return Ok(result.Data);
        }

        [HttpPatch("agregar-item/{id}")]
        public async Task<IActionResult> AgregarItemToAlquiler(int id, [FromBody] AddItemToAlquilerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _alquileresService.AgregarItemAlquiler(id, dto);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return NoContent();
        }

        [HttpPatch("{id}/item/cantidad")]
        public async Task<IActionResult> ModificarCantidadItem(int id, [FromBody] ModifyItemQuantityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _alquileresService.ModificarCantidadItem(id, dto);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return NoContent();
        }

        [HttpDelete("{id}/item/{itemId}")]
        public async Task<IActionResult> EliminarItemDeAlquiler(int id, int itemId)
        {
            var result = await _alquileresService.EliminarItemDeAlquiler(id, itemId);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return NoContent();
        }

        [HttpPost("{idAlquiler}/pagos")]
        public async Task<IActionResult> RegistrarPago([FromRoute] int idAlquiler, [FromQuery] int mes, [FromQuery] int anio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _alquileresService.RegistrarPago(idAlquiler, mes, anio);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return Ok(result.Data);
        }

        [HttpPut("{id}/finalizar")]
        public async Task<IActionResult> FinalizarAlquiler(int id)
        {
            var result = await _alquileresService.FinalizarAlquiler(id);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return NoContent();
        }
    }
}

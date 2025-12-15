using APIClub.Domain.AlquilerArticulos;
using APIClub.Dtos.AlquilerArticulos;
using APIClub.Dtos.ItemsAlquiler;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAlquileresActivos()
        {
            var result = await _alquileresService.GetAlquileresActivos();

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
        public async Task<IActionResult> RegistrarPago(int idAlquiler)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _alquileresService.RegistrarPago(idAlquiler);

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

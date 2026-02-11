using APIClub.Application.Dtos.Articulos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIClub.Domain.AlquilerArticulos.UseCases;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArticulosController : ControllerBase
    {
        private readonly IManagmentArticulosService _articulosService;

        public ArticulosController(IManagmentArticulosService articulosService)
        {
            _articulosService = articulosService;
        }

        [HttpPost]
        public async Task<IActionResult> CargarArticulo([FromBody] CreateArticuloDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _articulosService.CargarArticulo(dto);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return Ok(result.Data);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllArticulos()
        {
            var result = await _articulosService.GetAllArticulos();

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return Ok(result.Data);
        }


        [HttpPatch("{id}/precio")]
        public async Task<IActionResult> UpdatePrecioArticulo(int id, [FromBody] int nuevoPrecio)
        {
            var result = await _articulosService.UpdatePrecioArticulo(id, nuevoPrecio);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return Ok(result.Data);
        }
    }
}

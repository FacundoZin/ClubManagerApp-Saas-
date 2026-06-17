using APIClub.Application.Dtos.Viajes.Create;
using APIClub.Application.Dtos.Viajes.Update;
using APIClub.Domain.ModuloGestionViajes.Repositories;
using APIClub.Domain.ModuloGestionViajes.useCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ViajesController : ControllerBase
    {
        private readonly IViajesServices _viajesService;
        private readonly IViajeReadRepository _viajeReadRepository;

        public ViajesController(IViajesServices viajesService, IViajeReadRepository viajeReadRepository)
        {
            _viajesService = viajesService;
            _viajeReadRepository = viajeReadRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ListarViajesDisponibles()
        {
            var result = await _viajesService.ListarViajesDisponibles();
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok(result.Data);
        }

        [HttpGet("variantes/{idViajeBase}")]
        public async Task<IActionResult> ListarVariantesDeViaje(int idViajeBase)
        {
            var result = await _viajesService.ListarVariantesDeViaje(idViajeBase);
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok(result.Data);
        }

        [HttpGet("completo/{idViajeBase}")]
        public async Task<IActionResult> VerViajeCompleto(int idViajeBase)
        {
            var result = await _viajesService.VerViajeCompleto(idViajeBase);
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateViaje([FromBody] CreateViajeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _viajesService.CreateViaje(dto);
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok(result.Data);
        }

        [HttpPost("variante")]
        public async Task<IActionResult> CreateVarianteViaje([FromBody] CreateVarianteViajeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _viajesService.CreateVarianteViaje(dto);
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok(result.Data);
        }

        [HttpPost("inscribir")]
        public async Task<IActionResult> InscribirPersonasAlViaje([FromBody] InsertInscriptoViajeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
 
            var result = await _viajesService.InscribirPersonasAlViaje(dto);
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok(result.Data);
        }

        [HttpPost("pago")]
        public async Task<IActionResult> ActualizarPagoDeViaje([FromBody] UpdatePagoViajeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
 
            var result = await _viajesService.ActualizarPagoDeViaje(dto.IdInscripto, dto.MontoAbonado, dto.NumeroRecibo);
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok();
        }

        [HttpPut("pago/editar")]
        public async Task<IActionResult> EditarPagoDeViaje([FromBody] EditPagoViajeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userNameClaim = User.FindFirst(ClaimTypes.Name);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var userName = userNameClaim?.Value ?? "Usuario desconocido";

            var result = await _viajesService.EditarPagoDeViaje(dto.IdInscripto, dto.NuevoMontoAbonado, dto.MotivoModificacion, userId, userName);
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok();
        }

        [HttpDelete("inscripcion/{idInscripto}")]
        public async Task<IActionResult> CancelarInscripcionDeViaje(int idInscripto)
        {
            var result = await _viajesService.CancelarInscripcionDeViaje(idInscripto);
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok(result.Data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateViaje([FromBody] UpdateViajeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _viajesService.UpdateViaje(dto);
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok(result.Data);
        }

        [HttpPut("variante")]
        public async Task<IActionResult> UpdateVarianteViaje([FromBody] UpdateVarianteViajeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _viajesService.UpdateVarianteViaje(dto);
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }
            return Ok(result.Data);
        }

        [HttpGet("combobox")]

        public async Task<IActionResult> GetComboBoxViajes()
        {
            var viajes = await _viajeReadRepository.GetComboBoxViajes();

            return Ok(viajes);
        }

        [HttpGet("{idViaje}/variantes/combobox")]
        public async Task<IActionResult> GetComboBoxVariantesViaje(int idViaje)
        {
            var variantesViajes = await _viajeReadRepository.GetComboBoxVariantesDeViaje(idViaje);

            return Ok(variantesViajes);
        }
    }
}

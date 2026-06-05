using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using APIClub.Infrastructure.Persistence.Data;
using APIClub.Application.Dtos.Reservas;
using APIClub.Application.Common;
using Microsoft.AspNetCore.Authorization;
using APIClub.Domain.ModuloReservasSalones.useCases;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalonController : ControllerBase
    {
        private readonly IReservasServices _SalonesServices;
        private readonly AppDbcontext _context;

        public SalonController(IReservasServices salonesServices, AppDbcontext context)
        {
            _SalonesServices = salonesServices;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllSalons()
        {
            var salones = await _context.Salones
                .AsNoTracking()
                .Select(s => new { s.Id, Nombre = s.Name })
                .ToListAsync();

            return Ok(salones);
        }

        [HttpGet("{idSalon}/reservas")]
        public async Task<IActionResult> GetReservasBySalon(int idSalon, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _SalonesServices.GetReservasBySalon(idSalon, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{salonId}/disponibilidad")]
        public async Task<IActionResult> GetDisponibilidad([FromQuery] DateOnly fecha, int salonId)
        {
            var result = await _SalonesServices.GetDisponibilidadFecha(fecha, salonId);

            if (!result.Exit)
                return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(result);
        }

        [HttpGet("{salonId}/reserva")]
        public async Task<IActionResult> GetReservaByFecha(DateOnly fecha, int salonId)
        {
            var result = await _SalonesServices.GetReservaByFechaAndSalon(fecha, salonId);

            if (!result.Exit)
                return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(result.Data);
        }

        [HttpGet("reservas/{reservaId}")]
        public async Task<IActionResult> GetReservaById(int reservaId)
        {
            var result = await _SalonesServices.GetReservaById(reservaId);

            if (!result.Exit)
                return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(result.Data);
        }

        [HttpPost("reserva")]
        public async Task<IActionResult> CrearReserva([FromBody] CreteReservaSalonDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _SalonesServices.RegistrarReservaSalon(dto);

            if (!result.Exit)
                return StatusCode(result.Errorcode, result.Errormessage);

            return Ok(result);
        }


        [HttpDelete("reserva/{reservaId}")]
        public async Task<IActionResult> CancelarReserva(int reservaId)
        {
            var result = await _SalonesServices.CancelarReservas(reservaId);

            if (!result.Exit) return StatusCode(result.Errorcode, result.Errormessage);

            return NoContent();
        }

        [HttpPatch("reservas/{reservaId}/pago")]
        public async Task<IActionResult> ActualizarPagoDeSalon(int reservaId, decimal montoAbonado)
        {
            var result = await _SalonesServices.RegistrarPagoDeSalon(reservaId, montoAbonado);

            if (!result.Exit) return StatusCode(result.Errorcode, result.Errormessage);

            return Ok();
        }
        

    }
}

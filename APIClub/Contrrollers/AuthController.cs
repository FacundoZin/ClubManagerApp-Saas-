using APIClub.Application.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;
using APIClub.Application.Common;
using APIClub.Domain.Auth.useCases;

namespace APIClub.Contrrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.Login(dto);

            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, new { message = result.Errormessage });
            }

            // Set cookie httpOnly
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true in production
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = result.Data.Expiracion
            };

            Response.Cookies.Append("auth_token", result.Data.Token, cookieOptions);

            return Ok();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth_token");
            return Ok(new { message = "secion finalizada correctamente" });
        }


        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            // El UserId se obtendrá de los claims del token (ClaimsPrincipal)
            // Esto requiere que el middleware de autenticación esté configurado y funcionando
            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized();
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var result = await _authService.GetCurrentUser(userId);
            
            if (!result.Exit)
            {
                return StatusCode(result.Errorcode, result.Errormessage);
            }

            return Ok(result.Data);
        }
    }
}

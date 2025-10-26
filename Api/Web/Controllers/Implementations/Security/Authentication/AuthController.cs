using System.Security.Claims;
using Business.Interfaces.Security.Authentication;
using Entity.Dtos.Login;
using Entity.Dtos.Security;
using Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Security.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserAuthenticationBusiness _authBusiness;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserAuthenticationBusiness authBusiness, ILogger<AuthController> logger)
        {
            _authBusiness = authBusiness;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<UserResponseDto>(
                    null, false, "Datos inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            try
            {
                var user = await _authBusiness.AuthenticateAsync(request.Username, request.Password);
                if (user == null)
                    return Unauthorized(new ApiResponse<UserResponseDto>(null, false, "Credenciales incorrectas", null));

                return Ok(new ApiResponse<UserResponseDto>(user, true, "Inicio de sesión exitoso", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en autenticación");
                return StatusCode(500, new ApiResponse<UserResponseDto>(null, false, "Error interno del servidor", null));
            }
        }

        [Authorize]
        [HttpPost("select-parking")]
        public async Task<ActionResult<ApiResponse<object>>> SelectParking([FromBody] ParkingSelectionDto request)
        {
            try
            {
                if (request.ParkingId <= 0)
                    return BadRequest(new ApiResponse<object>(null, false, "ParkingId inválido", null));

                //  Obtener ID del usuario autenticado desde el token actual
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new ApiResponse<object>(null, false, "Usuario no autenticado", null));

                int userId = int.Parse(userIdClaim);

                // Validar que el usuario tenga acceso a ese parking
                bool hasAccess = await _authBusiness.ValidateUserParkingAccessAsync(userId, request.ParkingId);
                if (!hasAccess)
                    return Forbid();

                // Generar nuevo token con el parkingId embebido
                var newToken = await _authBusiness.GenerateTokenWithParkingAsync(userId, request.ParkingId);

                return Ok(new ApiResponse<object>(
                    new { token = newToken },
                    true,
                    "Token actualizado con parking seleccionado",
                    null
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al seleccionar parking");
                return StatusCode(500, new ApiResponse<object>(null, false, "Error interno del servidor", null));
            }
        }



    }
}

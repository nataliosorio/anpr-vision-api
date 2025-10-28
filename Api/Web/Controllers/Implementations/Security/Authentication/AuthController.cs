using System.Security.Claims;
using Business.Implementations.Security.Authentication.Interfaces;
using Business.Interfaces.Security.Authentication;
using Data.Interfaces;
using Entity.Dtos.Login;
using Entity.Dtos.Security;
using Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Interfaces;

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

        // Paso 1: Login con 2FA
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authBusiness.LoginWith2FAAsync(request.Username, request.Password);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        // Paso 2: Verificar OTP y generar token
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerificationRequestDto dto)
        {
            var result = await _authBusiness.VerifyOtpAndGenerateTokenAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Paso 3: Seleccionar parking
        [Authorize]
        [HttpPost("select-parking")]
        public async Task<IActionResult> SelectParking([FromBody] ParkingSelectionDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var token = await _authBusiness.GenerateTokenWithParkingAsync(userId, dto.ParkingId);
            return Ok(new ApiResponse<object>(new { token }, true, "Token actualizado", null));
        }
    }

}

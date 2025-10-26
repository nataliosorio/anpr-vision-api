using Business.Interfaces.Security.PasswordRecovery;
using Entity.Dtos.Security;
using Entity.Dtos;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Security.PasswordRecovery
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordRecoveryController : ControllerBase
    {
        private readonly IPasswordRecoveryBusiness _passwordRecovery;
        private readonly ILogger<PasswordRecoveryController> _logger;

        public PasswordRecoveryController(IPasswordRecoveryBusiness passwordRecovery, ILogger<PasswordRecoveryController> logger)
        {
            _passwordRecovery = passwordRecovery;
            _logger = logger;
        }

        [HttpPost("request")]
        public async Task<IActionResult> Request([FromBody] ResetRequestDto request)
        {
            try
            {
                await _passwordRecovery.RequestPasswordResetAsync(request.Email);
                return Ok(new ApiResponse<object>(null, true, "Se ha enviado un código de recuperación a tu correo.", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error solicitando recuperación de contraseña");
                return BadRequest(new ApiResponse<object>(null, false, ex.Message, null));
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyCodeDto request)
        {
            try
            {
                bool valid = await _passwordRecovery.VerifyResetCodeAsync(request.Email, request.Code);
                return Ok(new ApiResponse<object>(new { valid }, valid, valid ? "Código válido" : "Código inválido", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando código de recuperación");
                return BadRequest(new ApiResponse<object>(null, false, ex.Message, null));
            }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset([FromBody] ResetPasswordDto request)
        {
            try
            {
                await _passwordRecovery.VerifyCodeAndResetPasswordAsync(request.Email, request.Code, request.NewPassword);
                return Ok(new ApiResponse<object>(null, true, "La contraseña ha sido restablecida correctamente.", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reseteando contraseña");
                return BadRequest(new ApiResponse<object>(null, false, ex.Message, null));
            }
        }
    }
}

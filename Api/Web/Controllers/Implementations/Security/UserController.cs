using Business.Implementations;
using Business.Implementations.Security;
using Business.Interfaces.Security;
using Entity.Dtos;
using Entity.Dtos.Security;
using Entity.Models;
using Entity.Models.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.BackgroundTasks;
using Utilities.Exceptions;

namespace Web.Controllers.Implementations.Security
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : RepositoryController<User, UserDto>
    {
        private readonly IUserBusiness _business;
        private readonly ILogger<UserController> _logger;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public UserController(IUserBusiness business, ILogger<UserController> logger, IBackgroundTaskQueue backgroundTaskQueue)
            : base(business)
        {
            _logger = logger;
            _business = business;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllByParking()
        {
            try
            {
                var result = await _business.GetAllByParkingAsync();
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpPost]
        public override async Task<ActionResult<UserDto>> Save(UserDto dto)
        {
            try
            {
                // Guardar usuario y asignar rol por defecto dentro del método Save del Business
                UserDto dtoSaved = await _business.Save(dto);

                _backgroundTaskQueue.Enqueue(async token =>
                {
                    try
                    {
                        _logger.LogInformation(" Iniciando tarea en segundo plano para enviar correo...");

                        // Simular demora
                        await Task.Delay(3000, token);
                        token.ThrowIfCancellationRequested();

                        if (string.IsNullOrWhiteSpace(dtoSaved.Email))
                        {
                            _logger.LogWarning("No se envió el correo porque el email está vacío para el usuario ID {UserId}", dtoSaved.Id);
                            return;
                        }

                        await _business.SendWelcomeEmailAsync(dtoSaved.Email);
                        _logger.LogInformation("Tarea completada: Correo enviado.");
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogWarning("Tarea cancelada antes de enviar el correo a {Email}", dtoSaved.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en la tarea en segundo plano al enviar correo a {Email}", dtoSaved.Email);
                    }
                });
                _logger.LogInformation(" Tarea encolada. El controlador sigue respondiendo.");

                var response = new ApiResponse<UserDto>(dtoSaved, true, "Registro almacenado exitosamente", null!);

                return new CreatedAtRouteResult(new { id = dtoSaved.Id }, response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<UserDto>>(null!, false, ex.Message.ToString(), null!);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("roles/{userId}")]
        public async Task<IActionResult> GetUserRolesWithState(int userId)
        {
            try
            {
                var roles = await _business.GetUserRolesAsync(userId);
                return Ok(new { success = true, data = roles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles con estado del usuario");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }


    }
}

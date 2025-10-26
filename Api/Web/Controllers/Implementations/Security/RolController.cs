using Business.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers.Implementations.Security
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : RepositoryController<Rol, RolDto>
    {
        private readonly IRolBusiness _rolBusiness;

        public RolController(IRolBusiness rolBusiness)
            : base(rolBusiness)
        {
            _rolBusiness = rolBusiness;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<RolDto>>> GetAllByParking()
        {
            try
            {
                var result = await _rolBusiness.GetAllByParkingAsync();
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
    }
}

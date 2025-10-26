using Business.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models;
using Entity.Models.Security;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Security
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolParkingUserController : RepositoryController<RolParkingUser, RolParkingUserDto>
    {
        private readonly IRolParkingUserBusiness _business;
        public RolParkingUserController(IRolParkingUserBusiness business)
            : base(business)
        {
            _business = business;
        }

        // Endpoint personalizado para GetAllJoinAsync
        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<RolParkingUserDto>>> GetAllJoinAsync()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<RolParkingUserDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<RolParkingUserDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<RolParkingUserDto>>(null, false, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}

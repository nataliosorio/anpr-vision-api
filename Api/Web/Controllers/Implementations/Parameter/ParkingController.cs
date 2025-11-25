using Business.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models;
using Entity.Models.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Parameter
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingController : RepositoryController<Parking, ParkingDto>
    {
        private readonly IParkingBusiness _business;
        public ParkingController(IParkingBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<ParkingDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<ParkingDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<ParkingDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<ParkingDto>>(null, false, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}

using Business.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models;
using Entity.Models.Parameter;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Parameter
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZonesController : RepositoryController<Zones, ZonesDto>
    {
        private readonly IZonesBusiness _business;
        public ZonesController(IZonesBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<ZonesDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<ZonesDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<ZonesDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<ZonesDto>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        //[HttpGet("by-parking/{parkingId}")]
        //public async Task<ActionResult<IEnumerable<ZonesDto>>> GetAllByParkingId([FromRoute]int parkingId)
        //{
        //    try
        //    {
        //        IEnumerable<ZonesDto> data = await _business.GetAllByParkingId(parkingId);
        //        if (data == null || !data.Any())
        //        {
        //            var responseNull = new ApiResponse<IEnumerable<ZonesDto>>(null, false, "Registro no encontrado", null);
        //            return NotFound(responseNull);
        //        }
        //        var response = new ApiResponse<IEnumerable<ZonesDto>>(data, true, "Ok", null);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        var response = new ApiResponse<IEnumerable<ZonesDto>>(null, false, ex.Message.ToString(), null);
        //        return StatusCode(StatusCodes.Status500InternalServerError, response);
        //    }
        //}
    }
}

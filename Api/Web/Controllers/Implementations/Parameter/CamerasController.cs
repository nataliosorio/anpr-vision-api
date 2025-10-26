using Business.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models;
using Entity.Models.Parameter;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Parameter
{
    [ApiController]
    [Route("api/[controller]")]
    public class CamerasController :RepositoryController<Camera, CameraDto>
    {
        private readonly ICamaraBusiness _business;
        public CamerasController(ICamaraBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<CameraDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<CameraDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<CameraDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<CameraDto>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        //[HttpGet("by-parking/{parkingId:int}")]
        //public async Task<ActionResult<ApiResponse<IEnumerable<CameraDto>>>> GetByParking(int parkingId)
        //{
        //    try
        //    {
        //        if (parkingId <= 0)
        //            return BadRequest(new ApiResponse<IEnumerable<CameraDto>>(null, false, "ParkingId inválido.", null));

        //        var data = await _business.GetByParkingAsync(parkingId);

        //        if (data == null || !data.Any())
        //        {
        //            var responseNull = new ApiResponse<IEnumerable<CameraDto>>(null, false, "No se encontraron cámaras para este parqueadero.", null);
        //            return NotFound(responseNull);
        //        }

        //        var response = new ApiResponse<IEnumerable<CameraDto>>(data, true, "Ok", null);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        var response = new ApiResponse<IEnumerable<CameraDto>>(null, false, ex.Message, null);
        //        return StatusCode(StatusCodes.Status500InternalServerError, response);
        //    }
        //}

    }
}

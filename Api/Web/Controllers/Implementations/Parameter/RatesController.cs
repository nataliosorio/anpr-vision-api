using Business.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models;
using Entity.Models.Parameter;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Parameter
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatesController : RepositoryController<Rates, RatesDto>
    {
        private readonly IRatesBusiness _business;
        public RatesController(IRatesBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<RatesDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<RatesDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<RatesDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<RatesDto>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        //[HttpGet("by-parking/{parkingId:int}")]
        //public async Task<ActionResult<IEnumerable<RatesDto>>> GetByParking(int parkingId)
        //{
        //    try
        //    {
        //        var data = await _business.GetByParkingAsync(parkingId);

        //        if (data == null || !data.Any())
        //        {
        //            var responseNull = new ApiResponse<IEnumerable<RatesDto>>(null, false, "No se encontraron tarifas para este parqueadero.", null);
        //            return NotFound(responseNull);
        //        }

        //        var response = new ApiResponse<IEnumerable<RatesDto>>(data, true, "Ok", null);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        var response = new ApiResponse<IEnumerable<RatesDto>>(null, false, ex.Message, null);
        //        return StatusCode(StatusCodes.Status500InternalServerError, response);
        //    }
        //}



    }
}

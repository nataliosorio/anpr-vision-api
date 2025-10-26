using Business.Interfaces.Parameter;
using Entity.Dtos.Dashboard;
using Entity.Dtos.Parameter;
using Entity.Models;
using Entity.Models.Parameter;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Parameter
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : RepositoryController<Slots, SlotsDto>
    {
        private readonly ISlotsBusiness _business;
        public SlotsController(ISlotsBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<SlotsDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<SlotsDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<SlotsDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<SlotsDto>>(null, false, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [HttpGet("by-sector/{sectorId}")]
        public async Task<ActionResult<IEnumerable<SlotsDto>>> GetAllBysectorsId([FromRoute] int sectorId)
        {
            try
            {
                IEnumerable<SlotsDto> data = await _business.GetAllBySectorId(sectorId);
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<SlotsDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<SlotsDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<SlotsDto>>(null, false, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("occupancy/global")]
        public async Task<ActionResult<ApiResponse<OccupancyDto>>> GetOccupancyGlobal()
        {
            try
            {
                var data = await _business.GetOccupancyGlobalAsync();
                return Ok(new ApiResponse<OccupancyDto>(data, true, "Ok", null));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<OccupancyDto>(null!, false, ex.Message, null));
            }
        }

        [HttpGet("by-parking/{parkingId:int}")]
        public async Task<ActionResult<IEnumerable<SlotsDto>>> GetAllByParkingId([FromRoute] int parkingId)
        {
            try
            {
                var data = await _business.GetAllByParkingIdAsync(parkingId);

                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<SlotsDto>>(null, false, "No se encontraron slots para este parqueadero.", null);
                    return NotFound(responseNull);
                }

                var response = new ApiResponse<IEnumerable<SlotsDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<SlotsDto>>(null, false, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


    }
}

using Business.Implementations;
using Business.Interfaces.Operational;
using Entity.Dtos.Dashboard;
using Entity.Dtos.Operational;
using Entity.Models;
using Entity.Models.Operational;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Operational
{
    public class RegisteredVehiclesController: RepositoryController<RegisteredVehicles, RegisteredVehiclesDto>
    {
        private readonly IRegisteredVehicleBusiness _business;
        public RegisteredVehiclesController(IRegisteredVehicleBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<RegisteredVehiclesDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<RegisteredVehiclesDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<RegisteredVehiclesDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<RegisteredVehiclesDto>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // ---------- NUEVOS ENDPOINTS ----------
        // GET api/registeredvehicles/current/total?parkingId=1
        [HttpGet("current/total")]
        public async Task<IActionResult> GetTotalCurrentlyParked([FromQuery] int parkingId)
        {
            try
            {
                var total = await _business.GetTotalCurrentlyParkedByParkingAsync(parkingId);
                return Ok(new ApiResponse<object>(new { total }, true, "Ok", null));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>(null!, false, ex.Message, null));
            }
        }

        // Web/Controllers/Implements/RegisteredVehicleController.cs

        [HttpGet("current/total-global")]
        public async Task<IActionResult> GetTotalCurrentlyParkedGlobal()
        {
            try
            {
                var total = await _business.GetTotalCurrentlyParkedAsync();
                return Ok(new ApiResponse<object>(new { total }, true, "Ok", null));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>(null!, false, ex.Message, null));
            }
        }

        [HttpGet("distribution/types/global")]
        public async Task<IActionResult> GetVehicleTypeDistributionGlobal([FromQuery] bool includeZeros = true)
        {
            try
            {
                var data = await _business.GetVehicleTypeDistributionGlobalAsync(includeZeros);
                return Ok(new ApiResponse<VehicleTypeDistributionDto>(data, true, "Ok", null));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<VehicleTypeDistributionDto>(null!, false, ex.Message, null));
            }
        }

        [HttpGet("occupancy/sectors/by-zone/{zoneId:int}")]
        public async Task<IActionResult> GetSectorOccupancyByZone([FromRoute] int zoneId)
        {
            try
            {
                var data = await _business.GetSectorOccupancyByZoneAsync(zoneId);
                return Ok(new ApiResponse<List<OccupancyItemDto>>(data, true, "Ok", null));
            }
            catch (ArgumentException aex)
            {
                return BadRequest(new ApiResponse<List<OccupancyItemDto>>(null!, false, aex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<List<OccupancyItemDto>>(null!, false, ex.Message, null));
            }
        }


        //[HttpGet("by-parking/{parkingId:int}")]
        //public async Task<IActionResult> GetByParking([FromRoute] int parkingId)
        //{
        //    try
        //    {
        //        var data = await _business.GetByParkingAsync(parkingId);

        //        if (data == null || !data.Any())
        //        {
        //            var responseNull = new ApiResponse<IEnumerable<RegisteredVehiclesDto>>(null, false, "No se encontraron vehículos para este parqueadero.", null);
        //            return NotFound(responseNull);
        //        }

        //        var response = new ApiResponse<IEnumerable<RegisteredVehiclesDto>>(data, true, "Ok", null);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        var response = new ApiResponse<IEnumerable<RegisteredVehiclesDto>>(null, false, ex.Message, null);
        //        return StatusCode(StatusCodes.Status500InternalServerError, response);
        //    }
        //}


    }
}

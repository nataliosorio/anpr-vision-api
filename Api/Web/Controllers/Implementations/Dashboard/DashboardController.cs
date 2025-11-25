using Business.Interfaces.Dashboard;
using Entity.Dtos.Dashboard;
using Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Dashboard
{
    [Authorize]

    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardBusiness _business;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(IDashboardBusiness business, ILogger<DashboardController> logger)
        {
            _business = business;
            _logger = logger;

        }
        //traer total de vehiculos estacionados por parking
        [HttpGet("current/parked-vehicles/total")]
        public async Task<IActionResult> GetTotalByParking([FromQuery] int parkingId)
        {
            var total = await _business.GetTotalCurrentlyParkedByParkingAsync(parkingId);
            return Ok(new ApiResponse<object>(new { total }, true, "Ok", null));
        }

        [HttpGet("current/total-global")]
        public async Task<IActionResult> GetTotalGlobal()
        {
            var total = await _business.GetTotalCurrentlyParkedGlobalAsync();
            return Ok(new ApiResponse<object>(new { total }, true, "Ok", null));
        }

        //traer ocupacion por tipos de vehículos 
        [HttpGet("distribution/types/global")]
        public async Task<IActionResult> GetDistribution([FromQuery] int parkingId,bool includeZeros = true)
        {
            var data = await _business.GetVehicleTypeDistributionGlobalAsync(parkingId,includeZeros);
            return Ok(new ApiResponse<VehicleTypeDistributionDto>(data, true, "Ok", null));
        }

        [HttpGet("occupancy/sectors/by-zone/{zoneId:int}")]
        public async Task<IActionResult> GetOccupancy([FromRoute] int zoneId)
        {
            var data = await _business.GetSectorOccupancyByZoneAsync(zoneId);
            return Ok(new ApiResponse<List<OccupancyItemDto>>(data, true, "Ok", null));
        }

        //ocupacion global con porcentajes
        [HttpGet("occupancy/global")]
        public async Task<ActionResult<ApiResponse<OccupancyDto>>> GetOccupancyGlobal([FromQuery] int parkingId)
        {
                var data = await _business.GetOccupancyGlobalAsync(parkingId);
                return Ok(new ApiResponse<OccupancyDto>(data, true, "Ok", null));
        }
    }
}

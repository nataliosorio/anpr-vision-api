using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Dashboard;

namespace Business.Interfaces.Dashboard
{
    public interface IDashboardBusiness
    {
        Task<int> GetTotalCurrentlyParkedByParkingAsync(int parkingId);
        Task<int> GetTotalCurrentlyParkedGlobalAsync();
        Task<VehicleTypeDistributionDto> GetVehicleTypeDistributionGlobalAsync(int parkingId,bool includeZeros = true);
        Task<List<OccupancyItemDto>> GetSectorOccupancyByZoneAsync(int zoneId);

        Task<OccupancyDto> GetOccupancyGlobalAsync(int parkingId);
    }
}

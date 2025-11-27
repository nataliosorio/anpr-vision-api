using Entity.Dtos.Dashboard;
using Entity.Dtos.Operational;
using Entity.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.Operational
{
    public interface IRegisteredVehiclesData : IRepositoryData<RegisteredVehicles>
    {
        Task<bool> AnyActiveRegisteredVehicleInSlotAsync(int slotId);

        Task<IEnumerable<RegisteredVehiclesDto>> GetAllJoinAsync();

        Task<int> GetTotalCurrentlyParkedByParkingAsync(int parkingId);
        Task<int> GetTotalCurrentlyParkedAsync();
        Task<VehicleTypeDistributionDto> GetVehicleTypeDistributionGlobalAsync(bool includeZeros = true);
        Task<List<OccupancyItemDto>> GetSectorOccupancyByZoneAsync(int zoneId);
        Task<IEnumerable<RegisteredVehiclesDto>> GetByParkingAsync(int parkingId);
        Task<RegisteredVehicles?> GetActiveRegisterByVehicleIdAsync(int vehicleId);
        Task<RegisteredVehicles?> GetFullByIdAsync(int id);

    }
}

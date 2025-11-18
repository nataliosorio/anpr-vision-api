using Data.Implementations;
using Entity.Dtos.Dashboard;
using Entity.Dtos.Operational;
using Entity.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Operational
{
    public interface IRegisteredVehicleBusiness : IRepositoryBusiness<RegisteredVehicles, RegisteredVehiclesDto>
    {
        Task<IEnumerable<RegisteredVehiclesDto>> GetAllJoinAsync();
        Task<int> GetTotalCurrentlyParkedByParkingAsync(int parkingId);
        Task<int> GetTotalCurrentlyParkedAsync();
        Task<VehicleTypeDistributionDto> GetVehicleTypeDistributionGlobalAsync(bool includeZeros = true);

        Task<List<OccupancyItemDto>> GetSectorOccupancyByZoneAsync(int zoneId);
        Task<IEnumerable<RegisteredVehiclesDto>> GetByParkingAsync(int parkingId);
        Task<RegisteredVehiclesDto> RegisterVehicleWithSlotAsync(int vehicleId, int parkingId);
        Task<RegisteredVehiclesDto> RegisterVehicleExitAsync(int vehicleId);


        //  método para el registro manual por placa 
        Task<RegisteredVehiclesDto> ManualRegisterVehicleEntryAsync(ManualVehicleEntryDto dto);

    }
}

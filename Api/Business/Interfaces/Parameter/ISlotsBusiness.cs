using Entity.Dtos.Dashboard;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Parameter
{
    public interface ISlotsBusiness : IRepositoryBusiness<Slots, SlotsDto>
    {
        Task<IEnumerable<SlotsDto>> GetAllJoinAsync();
        Task<IEnumerable<SlotsDto>> GetAllBySectorId(int sectorId);
        Task<OccupancyDto> GetOccupancyGlobalAsync();
        Task<IEnumerable<SlotsDto>> GetAllByParkingIdAsync(int parkingId);
        Task<Slots> AssignAvailableSlotAsync(int typeVehicleId, int parkingId);


    }
}

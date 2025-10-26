using Entity.Dtos.Operational;
using Entity.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.Operational
{
    public interface IVehicleData: IRepositoryData<Vehicle>
    {
        Task<IEnumerable<VehicleDto>> GetAllJoinAsync();
        Task<RegisteredVehicles?> GetActiveRegisteredVehicleBySlotAsync(int slotId);
        Task<Vehicle?> GetVehicleByPlate(string plate);

    }
}

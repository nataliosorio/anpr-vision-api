using Entity.Dtos.Operational;
using Entity.Dtos.vehicle;
using Entity.Models.Operational;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IVehicleBusiness : IRepositoryBusiness<Vehicle, VehicleDto>
    {


        Task<IEnumerable<VehicleDto>> GetAllJoinAsync();
        // Nuevo método para registrar vehículo + slot automáticamente
        Task<RegisteredVehiclesDto?> GetActiveVehicleBySlotAsync(int slotId);
        Task<VehicleDto> GetVehicleByPlate(string plate);
        //Task<IEnumerable<VehicleDto>> GetVehiclesByClientIdAsync(int clientId);

        Task<IEnumerable<VehicleWithStatusDto>> GetVehiclesWithStatusByClientIdAsync(int clientId);





    }
}

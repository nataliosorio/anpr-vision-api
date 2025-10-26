using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Parameter
{
    public interface ISectorsBusiness : IRepositoryBusiness<Sectors, SectorsDto>
    {
        Task<IEnumerable<SectorsDto>> GetAllJoinAsync();
        Task<IEnumerable<SectorsDto>> GetAllByZoneId(int zoneId);
        Task<List<Sectors>> GetSectorsByVehicleTypeAsync(int vehicleTypeId , int parkingId);
    }
}

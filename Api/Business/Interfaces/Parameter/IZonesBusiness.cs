using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Parameter
{
    public interface IZonesBusiness : IRepositoryBusiness<Zones, ZonesDto>
    {
        Task<IEnumerable<ZonesDto>> GetAllJoinAsync();
        //Task<IEnumerable<ZonesDto>> GetAllByParkingId(int parkingId);
    }
}

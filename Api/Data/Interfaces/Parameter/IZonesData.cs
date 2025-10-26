using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.Parameter
{
    public interface IZonesData : IRepositoryData<Zones>
    {
        Task<IEnumerable<ZonesDto>> GetAllJoinAsync();
        //Task<IEnumerable<Zones>> GetAllByParkingId(int parkingId);
    }
}

using Entity.Dtos.Dashboard;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.Parameter
{
    public interface ISlotsData : IRepositoryData<Slots>
    {
       Task<IEnumerable<SlotsDto>> GetAllJoinAsync();
        Task<IEnumerable<Slots>> GetAllBySectorId(int sectorId);

        Task<bool> AnyAsync(Expression<Func<Slots, bool>> predicate); // 👈
        Task<int> CountExistingBySectorAsync(int sectorId);
        Task<OccupancyDto> GetOccupancyGlobalAsync();

        Task<IEnumerable<SlotsDto>> GetAllByParkingIdAsync(int parkingId);

    }
}

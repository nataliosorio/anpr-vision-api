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
    public interface IParkingData : IRepositoryData<Parking>
    {

        Task<bool> ExistsAsync(Expression<Func<Parking, bool>> predicate);
        //Task<bool> ExistsAsync<T>(Func<object, bool> value);
        Task<IEnumerable<ParkingDto>> GetAllJoinAsync();
    }
}

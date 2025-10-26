using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.Parameter
{
    public interface IMembershipsData : IRepositoryData<Memberships>
    {
        Task<IEnumerable<MembershipsDto>> GetAllJoinAsync();

        Task<bool> ExistsAsync<T>(Func<object, bool> value);

    }
}

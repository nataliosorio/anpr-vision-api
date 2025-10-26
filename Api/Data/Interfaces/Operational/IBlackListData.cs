using Entity.Dtos.Operational;
using Entity.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.Operational
{
    public interface IBlackListData : IRepositoryData<BlackList>
    {
        Task<IEnumerable<BlackListDto>> GetAllJoinAsync();


    }
}

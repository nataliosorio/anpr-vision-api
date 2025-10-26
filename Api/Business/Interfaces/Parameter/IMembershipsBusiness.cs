using Data.Implementations;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Parameter
{
    public interface IMembershipsBusiness : IRepositoryBusiness<Memberships, MembershipsDto>
    {
        Task<IEnumerable<MembershipsDto>> GetAllJoinAsync();

    }
}

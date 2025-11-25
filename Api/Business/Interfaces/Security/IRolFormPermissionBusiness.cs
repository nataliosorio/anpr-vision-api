using Entity.Dtos.Security;
using Entity.DtoSpecific.RolFormPermission;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Security
{
    public interface IRolFormPermissionBusiness : IRepositoryBusiness<RolFormPermission, RolFormPermissionDto>
    {

        Task<IEnumerable<RolFormPermissionDto>> GetAllJoinAsync();

        Task<RolFormPermissionGroupedDto?> GetAllByRolId(int rolId);
        Task<IEnumerable<RolFormPermissionGroupedDto>> GetAllGroupedAsync();

    }

}

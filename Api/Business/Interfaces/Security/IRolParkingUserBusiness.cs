using Entity.Dtos.Security;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Security
{
    public interface IRolParkingUserBusiness : IRepositoryBusiness<RolParkingUser, RolParkingUserDto>
    {
        public Task<IEnumerable<RolParkingUserDto>> GetAllJoinAsync();
        Task<bool> ExistsAsync(int userId, int roleId);
        Task<IEnumerable<RolParkingUserDto>> GetByUserIdAsync(int userId);

    }
}

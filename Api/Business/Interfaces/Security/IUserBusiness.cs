using Entity.Dtos.Access;
using Entity.Dtos.Security;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Security
{
    public interface IUserBusiness : IRepositoryBusiness<User, UserDto>
    {
        Task AssignDefaultRoleAsync(int userId);
        Task SendWelcomeEmailAsync(string to);

        Task<List<UserRoleByParkingDto>> GetUserRolesAsync(int userId);

        Task<UserAccessDto> GetUserAccessAsync(int userId, bool includePermissions = true, bool includeForms = true);
        Task<IEnumerable<UserDto>> GetAllByParkingAsync();



    }
}

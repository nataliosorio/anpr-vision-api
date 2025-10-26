using Entity.Dtos.Access;
using Entity.Dtos.Security;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IUserData : IRepositoryData<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);

        Task<User?> GetUserByEmailsync(string email);

        Task<List<string>> GetUserRoleAsync(int userId);

        //Task<List<UserRoleStatusDto>> GetUserRolesAsync(int userId);

        Task<UserAccessDto> GetUserAccessAsync(int userId);

        Task<List<UserRoleByParkingDto>> GetUserRolesAsync(int userId);
        Task<IEnumerable<User>> GetAllByParkingAsync();
        Task<User?> GetByPersonIdAsync(int personId);


    }
}

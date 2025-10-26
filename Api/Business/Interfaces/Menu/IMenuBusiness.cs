using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Access;

namespace Business.Interfaces.Menu
{
    public interface IMenuBusiness
    {
        Task<UserAccessDto> GetUserMenuAsync(int userId, int parkingId);
    }
}

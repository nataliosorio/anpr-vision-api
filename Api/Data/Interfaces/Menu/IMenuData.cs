using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Access;

namespace Data.Interfaces.Menu
{
    public interface IMenuData
    {
        Task<UserAccessDto> GetUserMenuAsync(int userId, int parkingId);
    }
}

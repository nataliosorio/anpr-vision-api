using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces.Menu;
using Data.Interfaces.Menu;
using Entity.Dtos.Access;

namespace Business.Implementations.Menu
{
    public class MenuBusiness : IMenuBusiness
    {
        private readonly IMenuData _repository;

        public MenuBusiness(IMenuData repository)
        {
            _repository = repository;
        }

        public async Task<UserAccessDto> GetUserMenuAsync(int userId, int parkingId)
        {
            return await _repository.GetUserMenuAsync(userId, parkingId);
        }
    }
}

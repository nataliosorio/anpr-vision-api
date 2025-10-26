using Entity.Dtos.Security;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Security
{
    public interface IRolBusiness : IRepositoryBusiness<Rol, RolDto>
    {
        Task<RolDto> GetByNameAsync(string name);
        Task<IEnumerable<RolDto>> GetAllByParkingAsync();
    }
}

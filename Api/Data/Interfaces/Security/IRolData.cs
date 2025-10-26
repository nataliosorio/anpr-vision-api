using Entity.Dtos.Security;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.Security
{
    public interface IRolData : IRepositoryData<Rol>
    {
        Task<Rol?> GetByNameAsync(string name);
        Task<IEnumerable<Rol>> GetAllByParkingAsync();

    }
}

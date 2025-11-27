using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.Security
{
    public interface IPersonData : IRepositoryData<Person>
    {
        Task<IEnumerable<Person>> GetAllByParkingAsync();
        Task<IEnumerable<Person>> GetUnlinkedAsync();
        Task<IEnumerable<Person>> GetPersonUnlinked();

    }
}

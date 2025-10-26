using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.Parameter
{
     public interface IClientData :IRepositoryData<Client>
    {

        //Task<IEnumerable<ClientDto>> GetAllJoinAsync();
        Task<Client?> GetClientWithVehiclesByPersonIdAsync(int personId);

        Task<IEnumerable<ClientDto>> GetAllJoinAsync();


    }
}

using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Parameter
{
    public interface IRatesBusiness : IRepositoryBusiness<Rates, RatesDto>
    {
        Task<IEnumerable<RatesDto>> GetAllJoinAsync();
        //Task<IEnumerable<RatesDto>> GetByParkingAsync(int parkingId);

    }
}

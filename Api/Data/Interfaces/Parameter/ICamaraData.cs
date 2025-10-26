using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;

namespace Data.Interfaces.Parameter
{

    public interface ICamaraData : IRepositoryData<Camera>
    {
        Task<IEnumerable<CameraDto>> GetAllJoinAsync();
        Task<bool> ExistsDuplicateAsync(CameraDto dto);
        Task<IEnumerable<CameraDto>> GetByParkingAsync(int parkingId);
    }
}


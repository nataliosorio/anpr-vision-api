using Entity.Dtos.Operational;
using Entity.Models.Operational;
using System.Linq.Expressions;
using Utilities.Exceptions;


namespace Business.Interfaces.Operational
{
    public interface IBlackListBusiness : IRepositoryBusiness<BlackList, BlackListDto>
    {
        Task<IEnumerable<BlackListDto>> GetAllJoinAsync();

    }
}

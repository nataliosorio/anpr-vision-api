using Business.Implementations;
using Business.Interfaces.Operational;
using Entity.Dtos.Operational;
using Entity.Models;
using Entity.Models.Operational;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Operational
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlackListController : RepositoryController<BlackList,BlackListDto>
    {
        private readonly IBlackListBusiness _business;
        public BlackListController(IBlackListBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
    
        public async Task<IActionResult> GetAllJoin()
        {
            var result = await _business.GetAllJoinAsync();
            var message = result.Any() ? "Consulta exitosa" : "No hay registros disponibles";

            return Ok(new ApiResponse<IEnumerable<BlackListDto>>(result, true, message, null));
        }

    }
}

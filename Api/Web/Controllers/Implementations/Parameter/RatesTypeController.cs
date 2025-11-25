using Business.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Parameter
{
    [Authorize]

    [ApiController]
    [Route("api/[controller]")]
    public class RatesTypeController : RepositoryController<RatesType, RatesTypeDto>
    {
        public RatesTypeController(IRatesTypeBusiness business)
            : base(business)
        {
        }
    }
}

using Business.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Parameter
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberShipTypeController: RepositoryController<MemberShipType, MemberShipTypeDto>
    {
        public MemberShipTypeController(IMemberShipTypeBusiness business)
            : base(business)
        {
        }
    }
}

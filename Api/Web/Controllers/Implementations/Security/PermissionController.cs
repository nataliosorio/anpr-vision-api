using Business.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models.Security;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Security
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : RepositoryController<Permission, PermissionDto>
    {
        public PermissionController(IPermissionBusiness business)
            : base(business)
        {
        }
    }
}

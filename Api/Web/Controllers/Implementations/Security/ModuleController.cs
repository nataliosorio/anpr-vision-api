using Business.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models.Security;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Security
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleController : RepositoryController<Module, ModuleDto>
    {
        public ModuleController(IModuleBusiness business)
            : base(business)
        {
        }
    }
}

using Business.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Security
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : RepositoryController<Form, FormDto>
    {
        public FormController(IFormBusiness business)
            : base(business)
        {
        }


    }
}

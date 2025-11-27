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
    public class TypeVehicleController : RepositoryController<TypeVehicle, TypeVehicleDto>
    {
        public TypeVehicleController(ITypeVehicleBusiness business)
            : base(business)
        {
        }
    }
}

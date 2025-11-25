using Business.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.DtoSpecific.RolFormPermission;
using Entity.Models;
using Entity.Models.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Security
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolFormPermissionController : RepositoryController<RolFormPermission, RolFormPermissionDto>
    {
        private readonly IRolFormPermissionBusiness _business;
        public RolFormPermissionController(IRolFormPermissionBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<RolFormPermissionDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<RolFormPermissionDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<RolFormPermissionDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<RolFormPermissionDto>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("byRol/{rolId:int}")]
        public async Task<ActionResult<IEnumerable<RolFormPermissionGroupedDto>>> GetAllByRolId(int rolId)
        {
            try
            {
                var data = await _business.GetAllByRolId(rolId);
                if (data == null)
                {
                    var responseNull = new ApiResponse<RolFormPermissionGroupedDto>(null, false, "No se encontraron permisos para el rol seleccionado.", null);
                    return NotFound(responseNull);
                }

                var response = new ApiResponse<RolFormPermissionGroupedDto>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<RolFormPermissionGroupedDto>(null, false, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("grouped")]
        public async Task<ActionResult<IEnumerable<RolFormPermissionGroupedDto>>> GetAllGrouped()
        {
            try
            {
                var data = await _business.GetAllGroupedAsync();

                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<RolFormPermissionGroupedDto>>(
                        null,
                        false,
                        "No se encontraron permisos configurados.",
                        null
                    );

                    return NotFound(responseNull);
                }

                var response = new ApiResponse<IEnumerable<RolFormPermissionGroupedDto>>(
                    data,
                    true,
                    "Ok",
                    null
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<RolFormPermissionGroupedDto>>(
                    null,
                    false,
                    ex.Message,
                    null
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}

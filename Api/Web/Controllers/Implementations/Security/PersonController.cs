using Business.Implementations.Security;
using Business.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers.Implementations.Security
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : RepositoryController<Person, PersonDto>
    {
        private readonly IPersonBusiness _bussines;
        public PersonController(IPersonBusiness business)
            : base(business)
        {
            _bussines = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetAllByParking()
        {
            try
            {
                var result = await _bussines.GetAllByParkingAsync();
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpGet("no-client")]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetUnlinked()
        {
            try
            {
                var result = await _bussines.GetUnlinkedAsync();
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpGet("no-user")]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetPersonUnlinked()
        {
            try
            {
                var result = await _bussines.GetUnlinkedAsync();
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }

    }
}

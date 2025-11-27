using Business.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models;
using Entity.Models.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Parameter
{
    [Authorize]

    public class ClientController : RepositoryController<Client, ClientDto>
    {
        private readonly IClientBusiness _business;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IClientBusiness business, ILogger<ClientController> logger)
           : base(business)
        {
            _business = business;
            _logger = logger;
        }

        

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllByParkingAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<ClientDto>>(null, false, "No se encontraron clientes en este parking.", null);
                    return NotFound(responseNull);
                }

                var response = new ApiResponse<IEnumerable<ClientDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<ClientDto>>(null, false, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}

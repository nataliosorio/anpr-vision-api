using Business.Interfaces.Operational;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Utilities.Interfaces.Ticket;

namespace Web.Controllers.Implementations.Operational
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly IRegisteredVehicleBusiness _vehicleBusiness;
        private readonly ITicketService _ticketService;

        public TicketController(
            IRegisteredVehicleBusiness vehicleBusiness,
            ITicketService ticketService)
        {
            _vehicleBusiness = vehicleBusiness;
            _ticketService = ticketService;
        }

        // GET api/tickets/{id}/pdf
        [HttpGet("{id:int}/pdf")]
        public async Task<IActionResult> GenerateTicket(int id)
        {
            try
            {
                // 1. Obtener DTO completo
                var dto = await _vehicleBusiness.GetRegisteredVehicleFullDtoAsync(id);

                if (dto == null)
                    return NotFound(new ApiResponse<object>(null, false, "Registro no encontrado.", null));

                // 2. Generar el PDF usando el servicio
                byte[] pdf = _ticketService.GenerateTicketPdf(dto);

                string fileName = $"ticket_{dto.Vehicle}_{dto.EntryDate:yyyyMMdd_HHmm}.pdf";
                return File(pdf, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new ApiResponse<object>(null, false, $"Error al generar el ticket: {ex.Message}", null));
            }
        }
    }
}

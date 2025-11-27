using System;
using Business.Interfaces.Operational;
using Entity.Dtos.Operational;
using Entity.Models;
using Entity.Models.Operational;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Interfaces;

namespace Web.Controllers.Implementations.Operational;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : RepositoryController<Notification, NotificationDto>
{
    private readonly INotificationBusiness _business;

    public NotificationController(INotificationBusiness business) : base(business)
    {
        _business = business;
    }

    [HttpGet("by-parking/{parkingId}")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByParking(int parkingId, [FromQuery] bool onlyUnread = false)
    {
        try
        {
            var data = await _business.GetByParkingAsync(parkingId, onlyUnread);

            if (data == null || data.Count == 0)
            {
                var responseNull = new ApiResponse<IEnumerable<NotificationDto>>(null, false, "Registro no encontrado", null);
                return NotFound(responseNull);
            }

            var response = new ApiResponse<IEnumerable<NotificationDto>>(data, true, "Ok", null);
            return Ok(response);
        }
        catch (Exception ex)
        {
            var response = new ApiResponse<IEnumerable<NotificationDto>>(null, false, ex.Message.ToString(), null);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPut("{id}/read")]
    public async Task<ActionResult> MarkAsRead(int id)
    {
        try
        {
            await _business.MarkAsReadAsync(id);
            var response = new ApiResponse<object>(
                new { Id = id, IsRead = true },
                true,
                "Notificación marcada como leída",
                null
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            var response = new ApiResponse<object>(
                null,
                false,
                ex.Message.ToString(),
                null
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPut("by-parking/{parkingId}/read-all")]
    public async Task<ActionResult> MarkAllAsReadByParking(int parkingId)
    {
        try
        {
            await _business.MarkAllAsReadByParkingAsync(parkingId);

            var response = new ApiResponse<object>(
                new { ParkingId = parkingId, Success = true },
                true,
                "Todas las notificaciones han sido marcadas como leídas",
                null
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            var response = new ApiResponse<object>(
                null,
                false,
                ex.Message,
                null
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<NotificationDto>> CreateAndNotify(NotificationDto dto)
    {
        try
        {
            var saved = await _business.CreateAndNotifyAsync(dto);
            var response = new ApiResponse<NotificationDto>(saved, true, "Notificación creada y enviada", null);
            return Ok(response);
        }
        catch (Exception ex)
        {
            var response = new ApiResponse<NotificationDto>(null, false, ex.Message.ToString(), null);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
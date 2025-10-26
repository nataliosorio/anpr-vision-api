using System;
using Business.Interfaces.Operational;
using Entity.Dtos.Operational;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;

namespace Web.Services;

public class SignalRNotificationDispatcher : INotificationDispatcher
{
    private readonly IHubContext<ParkingHub> _hubContext;

    public SignalRNotificationDispatcher(IHubContext<ParkingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendAsync(int parkingId, NotificationDto notification)
    {
        await _hubContext.Clients.Group(parkingId.ToString())
            .SendAsync("ReceiveNotification", notification);
    }
}

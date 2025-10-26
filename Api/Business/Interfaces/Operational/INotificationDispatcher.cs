using System;
using Entity.Dtos.Operational;

namespace Business.Interfaces.Operational;

public interface INotificationDispatcher
{
    Task SendAsync(int parkingId, NotificationDto notification);
}

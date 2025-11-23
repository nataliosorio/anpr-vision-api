using System;
using Entity.Dtos.Operational;
using Entity.Models.Operational;

namespace Business.Interfaces.Operational;

public interface INotificationBusiness : IRepositoryBusiness<Notification, NotificationDto>
{
    Task<List<NotificationDto>> GetByParkingAsync(int parkingId, bool onlyUnread = false);
    Task MarkAsReadAsync(int id);
    Task MarkAllAsReadByParkingAsync(int parkingId);
    Task<NotificationDto> CreateAndNotifyAsync(NotificationDto dto);
    Task<NotificationDto> EnqueueAndNotifyAsync(NotificationDto dto);
}

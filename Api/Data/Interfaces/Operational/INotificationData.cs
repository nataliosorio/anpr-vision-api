using System;
using Entity.Models.Operational;

namespace Data.Interfaces.Operational;

public interface INotificationData : IRepositoryData<Notification>
{
    Task<IEnumerable<Notification>> GetByParkingAsync(int parkingId, bool onlyUnread = false);
    Task MarkAsReadAsync(int id);
}

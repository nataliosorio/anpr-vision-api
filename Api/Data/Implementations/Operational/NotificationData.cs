using System;
using AutoMapper;
using Data.Interfaces.Operational;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Models.Operational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Operational;

public class NotificationData : RepositoryData<Notification>, INotificationData
{

    public NotificationData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext) : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
    {

    }

    public async Task<IEnumerable<Notification>> GetByParkingAsync(int parkingId, bool onlyUnread = false)
    {
        var query = _context.Notifications.AsQueryable()
            .Where(n => n.ParkingId == parkingId);

        if (onlyUnread)
            query = query.Where(n => !n.IsRead);

        return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
    }

        public async Task MarkAsReadAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
}

using System;
using AutoMapper;
using Business.Interfaces.Operational;
using Data.Interfaces.Operational;
using Entity.Dtos.Operational;
using Entity.Models.Operational;
using Microsoft.Extensions.Logging;
using Utilities.BackgroundTasks;

namespace Business.Implementations.Operational
{
    public class NotificationBusiness : RepositoryBusiness<Notification, NotificationDto>, INotificationBusiness
    {
        private readonly INotificationData _data;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationBusiness> _logger;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IBackgroundTaskQueue _taskQueue;

        public NotificationBusiness(
            INotificationData data,
            IMapper mapper,
            ILogger<NotificationBusiness> logger,
            INotificationDispatcher dispatcher,
            IBackgroundTaskQueue taskQueue)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
            _dispatcher = dispatcher;
            _taskQueue = taskQueue;
        }

        public async Task<List<NotificationDto>> GetByParkingAsync(int parkingId, bool onlyUnread = false)
        {
            var entities = await _data.GetByParkingAsync(parkingId, onlyUnread);
            return _mapper.Map<List<NotificationDto>>(entities);
        }

        public async Task MarkAsReadAsync(int id)
        {
            await _data.MarkAsReadAsync(id);
        }

        public async Task MarkAllAsReadByParkingAsync(int parkingId)
        {
            await _data.MarkAllAsReadByParkingAsync(parkingId);
        }

        /// <summary>
        /// Guarda la notificación y la envía inmediatamente.
        /// </summary>
        public async Task<NotificationDto> CreateAndNotifyAsync(NotificationDto dto)
        {
            var entity = _mapper.Map<Notification>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsRead = false;

            await _data.Save(entity);

            var savedDto = _mapper.Map<NotificationDto>(entity);

            // Enviar por SignalR
            await _dispatcher.SendAsync(entity.ParkingId ?? 0, savedDto);

            return savedDto;
        }

        /// <summary>
        /// Guarda la notificación y la encola para enviarse en background (no bloqueante).
        /// </summary>
        public async Task<NotificationDto> EnqueueAndNotifyAsync(NotificationDto dto)
        {
            Notification entity = _mapper.Map<Notification>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsRead = false;

            await _data.Save(entity);
            NotificationDto savedDto = _mapper.Map<NotificationDto>(entity);

            // Encolar envío en background
            _taskQueue.Enqueue(async _ =>
            {
                await _dispatcher.SendAsync(entity.ParkingId ?? 0, savedDto);
            });

            return savedDto;
        }
    }
}

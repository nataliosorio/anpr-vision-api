using AutoMapper;
using Business.Implementations.Operational;
using Business.Interfaces.Operational;
using Data.Interfaces.Operational;
using Entity.Dtos.Operational;
using Entity.Models.Operational;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace SecurityModel.Tests.Notification
{
    public class NotificationBusinessTests
    {
        private readonly Mock<INotificationData> _notificationDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<NotificationBusiness>> _loggerMock;
        private readonly NotificationBusiness _notificationBusiness;

        public NotificationBusinessTests()
        {
            _notificationDataMock = new Mock<INotificationData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<NotificationBusiness>>();

            _notificationBusiness = new NotificationBusiness(_notificationDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Save_ValidNotificationDto_ReturnsSavedNotificationDto()
        {
            // Arrange
            var notificationDto = new NotificationDto
            {
                Id = 1,
                Title = "Test Notification",
                Message = "This is a test message",
                UserId = 1,
                Asset = true
            };
            var notification = new Notification { Id = 1, Title = "Test Notification", Message = "This is a test message", UserId = 1 };
            var savedNotification = new Notification { Id = 1, Title = "Test Notification", Message = "This is a test message", UserId = 1 };
            var savedNotificationDto = new NotificationDto { Id = 1, Title = "Test Notification", Message = "This is a test message", UserId = 1 };

            _notificationDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Notification, bool>>>()))
                .ReturnsAsync(false);
            _notificationDataMock.Setup(x => x.Save(It.IsAny<Notification>()))
                .ReturnsAsync(savedNotification);
            _mapperMock.Setup(x => x.Map<Notification>(notificationDto))
                .Returns(notification);
            _mapperMock.Setup(x => x.Map<NotificationDto>(savedNotification))
                .Returns(savedNotificationDto);

            // Act
            var result = await _notificationBusiness.Save(notificationDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedNotificationDto);
            result.Title.Should().Be("Test Notification");
        }

        [Fact]
        public async Task GetById_ExistingNotificationId_ReturnsNotificationDto()
        {
            // Arrange
            var notificationId = 1;
            var notification = new Notification { Id = notificationId, Title = "Test Notification" };
            var notificationDto = new NotificationDto { Id = notificationId, Title = "Test Notification" };

            _notificationDataMock.Setup(x => x.GetById(notificationId))
                .ReturnsAsync(notification);
            _mapperMock.Setup(x => x.Map<NotificationDto>(notification))
                .Returns(notificationDto);

            // Act
            var result = await _notificationBusiness.GetById(notificationId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(notificationDto);
        }
    }
}
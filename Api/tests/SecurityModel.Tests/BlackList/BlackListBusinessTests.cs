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

namespace SecurityModel.Tests.BlackList
{
    public class BlackListBusinessTests
    {
        private readonly Mock<IBlackListData> _blackListDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BlackListBusiness>> _loggerMock;
        private readonly BlackListBusiness _blackListBusiness;

        public BlackListBusinessTests()
        {
            _blackListDataMock = new Mock<IBlackListData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BlackListBusiness>>();

            _blackListBusiness = new BlackListBusiness(_blackListDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Save_ValidBlackListDto_ReturnsSavedBlackListDto()
        {
            // Arrange
            var blackListDto = new BlackListDto
            {
                Id = 1,
                Plate = "ABC123",
                Reason = "Unauthorized vehicle",
                IsActive = true,
                Asset = true
            };
            var blackList = new BlackList { Id = 1, Plate = "ABC123", Reason = "Unauthorized vehicle", IsActive = true };
            var savedBlackList = new BlackList { Id = 1, Plate = "ABC123", Reason = "Unauthorized vehicle", IsActive = true };
            var savedBlackListDto = new BlackListDto { Id = 1, Plate = "ABC123", Reason = "Unauthorized vehicle", IsActive = true };

            _blackListDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<BlackList, bool>>>()))
                .ReturnsAsync(false);
            _blackListDataMock.Setup(x => x.Save(It.IsAny<BlackList>()))
                .ReturnsAsync(savedBlackList);
            _mapperMock.Setup(x => x.Map<BlackList>(blackListDto))
                .Returns(blackList);
            _mapperMock.Setup(x => x.Map<BlackListDto>(savedBlackList))
                .Returns(savedBlackListDto);

            // Act
            var result = await _blackListBusiness.Save(blackListDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedBlackListDto);
            result.Plate.Should().Be("ABC123");
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task GetById_ExistingBlackListId_ReturnsBlackListDto()
        {
            // Arrange
            var blackListId = 1;
            var blackList = new BlackList { Id = blackListId, Plate = "XYZ789" };
            var blackListDto = new BlackListDto { Id = blackListId, Plate = "XYZ789" };

            _blackListDataMock.Setup(x => x.GetById(blackListId))
                .ReturnsAsync(blackList);
            _mapperMock.Setup(x => x.Map<BlackListDto>(blackList))
                .Returns(blackListDto);

            // Act
            var result = await _blackListBusiness.GetById(blackListId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(blackListDto);
        }
    }
}
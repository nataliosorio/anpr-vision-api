using AutoMapper;
using Business.Implementations.Parameter;
using Business.Interfaces.Parameter;
using Data.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace SecurityModel.Tests.Memberships
{
    public class MembershipsBusinessTests
    {
        private readonly Mock<IMembershipsData> _membershipsDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<MembershipsBusiness>> _loggerMock;
        private readonly MembershipsBusiness _membershipsBusiness;

        public MembershipsBusinessTests()
        {
            _membershipsDataMock = new Mock<IMembershipsData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<MembershipsBusiness>>();

            _membershipsBusiness = new MembershipsBusiness(_membershipsDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Save_ValidMembershipsDto_ReturnsSavedMembershipsDto()
        {
            // Arrange
            var membershipsDto = new MembershipsDto
            {
                Id = 1,
                Name = "Premium Membership",
                Description = "Premium membership package",
                ClientId = 1,
                Asset = true
            };
            var memberships = new Memberships { Id = 1, Name = "Premium Membership", Description = "Premium membership package", ClientId = 1 };
            var savedMemberships = new Memberships { Id = 1, Name = "Premium Membership", Description = "Premium membership package", ClientId = 1 };
            var savedMembershipsDto = new MembershipsDto { Id = 1, Name = "Premium Membership", Description = "Premium membership package", ClientId = 1 };

            _membershipsDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Memberships, bool>>>()))
                .ReturnsAsync(false);
            _membershipsDataMock.Setup(x => x.Save(It.IsAny<Memberships>()))
                .ReturnsAsync(savedMemberships);
            _mapperMock.Setup(x => x.Map<Memberships>(membershipsDto))
                .Returns(memberships);
            _mapperMock.Setup(x => x.Map<MembershipsDto>(savedMemberships))
                .Returns(savedMembershipsDto);

            // Act
            var result = await _membershipsBusiness.Save(membershipsDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedMembershipsDto);
            result.Name.Should().Be("Premium Membership");
        }

        [Fact]
        public async Task GetById_ExistingMembershipsId_ReturnsMembershipsDto()
        {
            // Arrange
            var membershipsId = 1;
            var memberships = new Memberships { Id = membershipsId, Name = "Test Membership" };
            var membershipsDto = new MembershipsDto { Id = membershipsId, Name = "Test Membership" };

            _membershipsDataMock.Setup(x => x.GetById(membershipsId))
                .ReturnsAsync(memberships);
            _mapperMock.Setup(x => x.Map<MembershipsDto>(memberships))
                .Returns(membershipsDto);

            // Act
            var result = await _membershipsBusiness.GetById(membershipsId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(membershipsDto);
        }
    }
}
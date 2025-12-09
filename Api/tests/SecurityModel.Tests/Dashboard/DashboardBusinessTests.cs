using Business.Implementations.Dashboard;
using Business.Interfaces.Dashboard;
using Data.Interfaces.Dashboard;
using Entity.Dtos.Dashboard;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace SecurityModel.Tests.Dashboard
{
    public class DashboardBusinessTests
    {
        private readonly Mock<IDashboardRepository> _dashboardRepositoryMock;
        private readonly Mock<ILogger<DashboardBusiness>> _loggerMock;
        private readonly DashboardBusiness _dashboardBusiness;

        public DashboardBusinessTests()
        {
            _dashboardRepositoryMock = new Mock<IDashboardRepository>();
            _loggerMock = new Mock<ILogger<DashboardBusiness>>();

            _dashboardBusiness = new DashboardBusiness(
                _dashboardRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        #region GetTotalCurrentlyParkedByParkingAsync Tests

        [Fact]
        public async Task GetTotalCurrentlyParkedByParkingAsync_ValidParkingId_ReturnsVehicleCount()
        {
            // Arrange
            var parkingId = 1;
            var expectedCount = 25;

            _dashboardRepositoryMock.Setup(x => x.GetTotalCurrentlyParkedByParkingAsync(parkingId))
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _dashboardBusiness.GetTotalCurrentlyParkedByParkingAsync(parkingId);

            // Assert
            result.Should().Be(expectedCount);
            _dashboardRepositoryMock.Verify(x => x.GetTotalCurrentlyParkedByParkingAsync(parkingId), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetTotalCurrentlyParkedByParkingAsync_InvalidParkingId_ThrowsArgumentException(int invalidParkingId)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => 
                await _dashboardBusiness.GetTotalCurrentlyParkedByParkingAsync(invalidParkingId));

            exception.Message.Should().Contain("parkingId inválido");
        }

        #endregion

        #region GetOccupancyGlobalAsync Tests

        [Fact]
        public async Task GetOccupancyGlobalAsync_ValidParkingId_ReturnsOccupancyData()
        {
            // Arrange
            var parkingId = 1;
            var expectedOccupancy = new OccupancyDto
            {
                Available = 50,
                Occupied = 25,
                Percentage = 33.33m
            };

            _dashboardRepositoryMock.Setup(x => x.GetOccupancyGlobalAsync(parkingId))
                .ReturnsAsync(expectedOccupancy);

            // Act
            var result = await _dashboardBusiness.GetOccupancyGlobalAsync(parkingId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedOccupancy);
            result.Occupied.Should().Be(25);
            result.Percentage.Should().Be(33.33m);
        }

        #endregion

        #region GetVehicleTypeDistributionGlobalAsync Tests

        [Fact]
        public async Task GetVehicleTypeDistributionGlobalAsync_ValidParkingId_ReturnsDistribution()
        {
            // Arrange
            var parkingId = 1;
            var expectedDistribution = new VehicleTypeDistributionDto
            {
                Slices = new List<VehicleTypeSliceDto>
                {
                    new VehicleTypeSliceDto { TypeName = "Carros", Count = 50 },
                    new VehicleTypeSliceDto { TypeName = "Motos", Count = 25 }
                }
            };

            _dashboardRepositoryMock.Setup(x => x.GetVehicleTypeDistributionByParkingAsync(parkingId, true))
                .ReturnsAsync(expectedDistribution);

            // Act
            var result = await _dashboardBusiness.GetVehicleTypeDistributionGlobalAsync(parkingId);

            // Assert
            result.Should().NotBeNull();
            result.Slices.Should().HaveCount(2);
            result.Slices.First().TypeName.Should().Be("Carros");
        }

        #endregion
    }
}
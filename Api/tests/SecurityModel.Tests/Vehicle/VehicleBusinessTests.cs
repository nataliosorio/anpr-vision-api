using AutoMapper;
using Business.Implementations.Operational;
using Business.Interfaces;
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

namespace SecurityModel.Tests.Vehicle
{
    public class VehicleBusinessTests
    {
        private readonly Mock<IVehicleData> _vehicleDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<VehicleBusiness>> _loggerMock;
        private readonly VehicleBusiness _vehicleBusiness;

        public VehicleBusinessTests()
        {
            _vehicleDataMock = new Mock<IVehicleData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<VehicleBusiness>>();

            _vehicleBusiness = new VehicleBusiness(_vehicleDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        #region GetVehicleByPlate Tests

        [Fact]
        public async Task GetVehicleByPlate_ExistingPlate_ReturnsVehicleDto()
        {
            // Arrange
            var plate = "ABC123";
            var vehicle = new Vehicle { Id = 1, Plate = plate, Asset = true };
            var vehicleDto = new VehicleDto { Id = 1, Plate = plate, Asset = true };

            _vehicleDataMock.Setup(x => x.GetVehicleByPlate(plate))
                .ReturnsAsync(vehicle);
            _mapperMock.Setup(x => x.Map<VehicleDto>(vehicle))
                .Returns(vehicleDto);

            // Act
            var result = await _vehicleBusiness.GetVehicleByPlate(plate);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(vehicleDto);
            result.Plate.Should().Be(plate);
        }

        [Fact]
        public async Task GetVehicleByPlate_NonExistingPlate_ReturnsNull()
        {
            // Arrange
            var plate = "NONEXISTENT";

            _vehicleDataMock.Setup(x => x.GetVehicleByPlate(plate))
                .ReturnsAsync((Vehicle?)null);

            // Act
            var result = await _vehicleBusiness.GetVehicleByPlate(plate);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region Save Tests

        [Fact]
        public async Task Save_ValidVehicleDto_ReturnsSavedVehicleDto()
        {
            // Arrange
            var vehicleDto = new VehicleDto
            {
                Id = 1,
                Plate = "XYZ789",
                TypeVehicleId = 1,
                Asset = true
            };
            var vehicle = new Vehicle { Id = 1, Plate = "XYZ789", TypeVehicleId = 1 };
            var savedVehicle = new Vehicle { Id = 1, Plate = "XYZ789", TypeVehicleId = 1 };
            var savedVehicleDto = new VehicleDto { Id = 1, Plate = "XYZ789", TypeVehicleId = 1 };

            _vehicleDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Vehicle, bool>>>()))
                .ReturnsAsync(false);
            _vehicleDataMock.Setup(x => x.Save(It.IsAny<Vehicle>()))
                .ReturnsAsync(savedVehicle);
            _mapperMock.Setup(x => x.Map<Vehicle>(vehicleDto))
                .Returns(vehicle);
            _mapperMock.Setup(x => x.Map<VehicleDto>(savedVehicle))
                .Returns(savedVehicleDto);

            // Act
            var result = await _vehicleBusiness.Save(vehicleDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedVehicleDto);
        }

        #endregion
    }
}
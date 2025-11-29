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

namespace SecurityModel.Tests.RegisteredVehicle
{
    public class RegisteredVehicleBusinessTests
    {
        private readonly Mock<IRegisteredVehiclesData> _registeredVehicleDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<RegisteredVehicleBusiness>> _loggerMock;
        private readonly RegisteredVehicleBusiness _registeredVehicleBusiness;

        public RegisteredVehicleBusinessTests()
        {
            _registeredVehicleDataMock = new Mock<IRegisteredVehiclesData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RegisteredVehicleBusiness>>();

            _registeredVehicleBusiness = new RegisteredVehicleBusiness(
                _registeredVehicleDataMock.Object, 
                _mapperMock.Object, 
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Save_ValidRegisteredVehicleDto_ReturnsSavedRegisteredVehicleDto()
        {
            // Arrange
            var registeredVehicleDto = new RegisteredVehiclesDto
            {
                Id = 1,
                VehicleId = 1,
                ClientId = 1,
                RegistrationDate = DateTime.UtcNow,
                Asset = true
            };
            var registeredVehicle = new RegisteredVehicles { Id = 1, VehicleId = 1, ClientId = 1 };
            var savedRegisteredVehicle = new RegisteredVehicles { Id = 1, VehicleId = 1, ClientId = 1 };
            var savedRegisteredVehicleDto = new RegisteredVehiclesDto { Id = 1, VehicleId = 1, ClientId = 1 };

            _registeredVehicleDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<RegisteredVehicles, bool>>>()))
                .ReturnsAsync(false);
            _registeredVehicleDataMock.Setup(x => x.Save(It.IsAny<RegisteredVehicles>()))
                .ReturnsAsync(savedRegisteredVehicle);
            _mapperMock.Setup(x => x.Map<RegisteredVehicles>(registeredVehicleDto))
                .Returns(registeredVehicle);
            _mapperMock.Setup(x => x.Map<RegisteredVehiclesDto>(savedRegisteredVehicle))
                .Returns(savedRegisteredVehicleDto);

            // Act
            var result = await _registeredVehicleBusiness.Save(registeredVehicleDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedRegisteredVehicleDto);
        }

        [Fact]
        public async Task GetById_ExistingRegisteredVehicleId_ReturnsRegisteredVehicleDto()
        {
            // Arrange
            var registeredVehicleId = 1;
            var registeredVehicle = new RegisteredVehicles { Id = registeredVehicleId, VehicleId = 1 };
            var registeredVehicleDto = new RegisteredVehiclesDto { Id = registeredVehicleId, VehicleId = 1 };

            _registeredVehicleDataMock.Setup(x => x.GetById(registeredVehicleId))
                .ReturnsAsync(registeredVehicle);
            _mapperMock.Setup(x => x.Map<RegisteredVehiclesDto>(registeredVehicle))
                .Returns(registeredVehicleDto);

            // Act
            var result = await _registeredVehicleBusiness.GetById(registeredVehicleId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(registeredVehicleDto);
        }
    }
}
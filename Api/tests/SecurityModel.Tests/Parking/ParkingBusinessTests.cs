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

namespace SecurityModel.Tests.Parking
{
    public class ParkingBusinessTests
    {
        private readonly Mock<IParkingData> _parkingDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ParkingBusiness>> _loggerMock;
        private readonly ParkingBusiness _parkingBusiness;

        public ParkingBusinessTests()
        {
            _parkingDataMock = new Mock<IParkingData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ParkingBusiness>>();

            _parkingBusiness = new ParkingBusiness(_parkingDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        #region Save Tests

        [Fact]
        public async Task Save_ValidParkingDto_ReturnsSavedParkingDto()
        {
            // Arrange
            var parkingDto = new ParkingDto
            {
                Id = 1,
                Name = "Main Parking",
                Address = "123 Main St",
                Capacity = 100,
                Asset = true
            };
            var parking = new Parking { Id = 1, Name = "Main Parking", Address = "123 Main St", Capacity = 100 };
            var savedParking = new Parking { Id = 1, Name = "Main Parking", Address = "123 Main St", Capacity = 100 };
            var savedParkingDto = new ParkingDto { Id = 1, Name = "Main Parking", Address = "123 Main St", Capacity = 100 };

            _parkingDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Parking, bool>>>()))
                .ReturnsAsync(false);
            _parkingDataMock.Setup(x => x.Save(It.IsAny<Parking>()))
                .ReturnsAsync(savedParking);
            _mapperMock.Setup(x => x.Map<Parking>(parkingDto))
                .Returns(parking);
            _mapperMock.Setup(x => x.Map<ParkingDto>(savedParking))
                .Returns(savedParkingDto);

            // Act
            var result = await _parkingBusiness.Save(parkingDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedParkingDto);
            result.Name.Should().Be("Main Parking");
        }

        [Fact]
        public async Task Save_DuplicateName_ThrowsInvalidOperationException()
        {
            // Arrange
            var parkingDto = new ParkingDto
            {
                Name = "Existing Parking",
                Address = "456 Oak St",
                Capacity = 50
            };

            _parkingDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Parking, bool>>>()))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await _parkingBusiness.Save(parkingDto));

            exception.Message.Should().Contain("ya se encuentra registrado");
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_ExistingParkingId_ReturnsParkingDto()
        {
            // Arrange
            var parkingId = 1;
            var parking = new Parking { Id = parkingId, Name = "Test Parking" };
            var parkingDto = new ParkingDto { Id = parkingId, Name = "Test Parking" };

            _parkingDataMock.Setup(x => x.GetById(parkingId))
                .ReturnsAsync(parking);
            _mapperMock.Setup(x => x.Map<ParkingDto>(parking))
                .Returns(parkingDto);

            // Act
            var result = await _parkingBusiness.GetById(parkingId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(parkingDto);
        }

        [Fact]
        public async Task GetById_NonExistingParkingId_ReturnsNull()
        {
            // Arrange
            var parkingId = 999;

            _parkingDataMock.Setup(x => x.GetById(parkingId))
                .ReturnsAsync((Parking?)null);

            // Act
            var result = await _parkingBusiness.GetById(parkingId);

            // Assert
            result.Should().BeNull();
        }

        #endregion
    }
}
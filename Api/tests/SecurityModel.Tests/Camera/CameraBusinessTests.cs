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

namespace SecurityModel.Tests.Camera
{
    public class CameraBusinessTests
    {
        private readonly Mock<ICamaraData> _cameraDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CamaraBusiness>> _loggerMock;
        private readonly CamaraBusiness _cameraBusiness;

        public CameraBusinessTests()
        {
            _cameraDataMock = new Mock<ICamaraData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CamaraBusiness>>();

            _cameraBusiness = new CamaraBusiness(_cameraDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Save_ValidCameraDto_ReturnsSavedCameraDto()
        {
            // Arrange
            var cameraDto = new CameraDto
            {
                Id = 1,
                Name = "Entrance Camera",
                IpAddress = "192.168.1.100",
                ParkingId = 1,
                Asset = true
            };
            var camera = new Camera { Id = 1, Name = "Entrance Camera", IpAddress = "192.168.1.100", ParkingId = 1 };
            var savedCamera = new Camera { Id = 1, Name = "Entrance Camera", IpAddress = "192.168.1.100", ParkingId = 1 };
            var savedCameraDto = new CameraDto { Id = 1, Name = "Entrance Camera", IpAddress = "192.168.1.100", ParkingId = 1 };

            _cameraDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Camera, bool>>>()))
                .ReturnsAsync(false);
            _cameraDataMock.Setup(x => x.Save(It.IsAny<Camera>()))
                .ReturnsAsync(savedCamera);
            _mapperMock.Setup(x => x.Map<Camera>(cameraDto))
                .Returns(camera);
            _mapperMock.Setup(x => x.Map<CameraDto>(savedCamera))
                .Returns(savedCameraDto);

            // Act
            var result = await _cameraBusiness.Save(cameraDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedCameraDto);
            result.Name.Should().Be("Entrance Camera");
        }

        [Fact]
        public async Task GetById_ExistingCameraId_ReturnsCameraDto()
        {
            // Arrange
            var cameraId = 1;
            var camera = new Camera { Id = cameraId, Name = "Test Camera" };
            var cameraDto = new CameraDto { Id = cameraId, Name = "Test Camera" };

            _cameraDataMock.Setup(x => x.GetById(cameraId))
                .ReturnsAsync(camera);
            _mapperMock.Setup(x => x.Map<CameraDto>(camera))
                .Returns(cameraDto);

            // Act
            var result = await _cameraBusiness.GetById(cameraId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(cameraDto);
        }
    }
}
using AutoMapper;
using Business.Implementations.Security;
using Business.Interfaces.Security;
using Data.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models.Security;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace SecurityModel.Tests.Module
{
    public class ModuleBusinessTests
    {
        private readonly Mock<IModuleData> _moduleDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ModuleBusiness>> _loggerMock;
        private readonly ModuleBusiness _moduleBusiness;

        public ModuleBusinessTests()
        {
            _moduleDataMock = new Mock<IModuleData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ModuleBusiness>>();

            _moduleBusiness = new ModuleBusiness(_moduleDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Save_ValidModuleDto_ReturnsSavedModuleDto()
        {
            // Arrange
            var moduleDto = new ModuleDto
            {
                Id = 1,
                Name = "Dashboard",
                Description = "Dashboard module",
                Asset = true
            };
            var module = new Module { Id = 1, Name = "Dashboard", Description = "Dashboard module" };
            var savedModule = new Module { Id = 1, Name = "Dashboard", Description = "Dashboard module" };
            var savedModuleDto = new ModuleDto { Id = 1, Name = "Dashboard", Description = "Dashboard module" };

            _moduleDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Module, bool>>>()))
                .ReturnsAsync(false);
            _moduleDataMock.Setup(x => x.Save(It.IsAny<Module>()))
                .ReturnsAsync(savedModule);
            _mapperMock.Setup(x => x.Map<Module>(moduleDto))
                .Returns(module);
            _mapperMock.Setup(x => x.Map<ModuleDto>(savedModule))
                .Returns(savedModuleDto);

            // Act
            var result = await _moduleBusiness.Save(moduleDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedModuleDto);
            result.Name.Should().Be("Dashboard");
        }

        [Fact]
        public async Task GetById_ExistingModuleId_ReturnsModuleDto()
        {
            // Arrange
            var moduleId = 1;
            var module = new Module { Id = moduleId, Name = "Test Module" };
            var moduleDto = new ModuleDto { Id = moduleId, Name = "Test Module" };

            _moduleDataMock.Setup(x => x.GetById(moduleId))
                .ReturnsAsync(module);
            _mapperMock.Setup(x => x.Map<ModuleDto>(module))
                .Returns(moduleDto);

            // Act
            var result = await _moduleBusiness.GetById(moduleId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(moduleDto);
        }
    }
}
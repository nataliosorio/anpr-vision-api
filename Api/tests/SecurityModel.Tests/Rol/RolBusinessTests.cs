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

namespace SecurityModel.Tests.Rol
{
    public class RolBusinessTests
    {
        private readonly Mock<IRolData> _rolDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<RolBusiness>> _loggerMock;
        private readonly RolBusiness _rolBusiness;

        public RolBusinessTests()
        {
            _rolDataMock = new Mock<IRolData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RolBusiness>>();

            _rolBusiness = new RolBusiness(_rolDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        #region GetByNameAsync Tests

        [Fact]
        public async Task GetByNameAsync_ExistingRoleName_ReturnsRole()
        {
            // Arrange
            var roleName = "Administrator";
            var rol = new Rol { Id = 1, Name = roleName, Asset = true };

            _rolDataMock.Setup(x => x.GetByNameAsync(roleName))
                .ReturnsAsync(rol);

            // Act
            var result = await _rolBusiness.GetByNameAsync(roleName);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(rol);
            result.Name.Should().Be(roleName);
        }

        [Fact]
        public async Task GetByNameAsync_NonExistingRoleName_ReturnsNull()
        {
            // Arrange
            var roleName = "NonExistentRole";

            _rolDataMock.Setup(x => x.GetByNameAsync(roleName))
                .ReturnsAsync((Rol?)null);

            // Act
            var result = await _rolBusiness.GetByNameAsync(roleName);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region Save Tests

        [Fact]
        public async Task Save_ValidRolDto_ReturnsSavedRolDto()
        {
            // Arrange
            var rolDto = new RolDto { Name = "NewRole", Asset = true };
            var rol = new Rol { Id = 1, Name = "NewRole", Asset = true };
            var savedRol = new Rol { Id = 1, Name = "NewRole", Asset = true };
            var savedRolDto = new RolDto { Id = 1, Name = "NewRole", Asset = true };

            _rolDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Rol, bool>>>()))
                .ReturnsAsync(false);
            _rolDataMock.Setup(x => x.Save(It.IsAny<Rol>()))
                .ReturnsAsync(savedRol);
            _mapperMock.Setup(x => x.Map<Rol>(rolDto))
                .Returns(rol);
            _mapperMock.Setup(x => x.Map<RolDto>(savedRol))
                .Returns(savedRolDto);

            // Act
            var result = await _rolBusiness.Save(rolDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedRolDto);
        }

        #endregion
    }
}
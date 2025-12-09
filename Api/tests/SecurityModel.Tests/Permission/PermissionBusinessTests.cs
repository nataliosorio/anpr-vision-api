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

namespace SecurityModel.Tests.Permission
{
    public class PermissionBusinessTests
    {
        private readonly Mock<IPermissionData> _permissionDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PermissionBusiness>> _loggerMock;
        private readonly PermissionBusiness _permissionBusiness;

        public PermissionBusinessTests()
        {
            _permissionDataMock = new Mock<IPermissionData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PermissionBusiness>>();

            _permissionBusiness = new PermissionBusiness(_permissionDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Save_ValidPermissionDto_ReturnsSavedPermissionDto()
        {
            // Arrange
            var permissionDto = new PermissionDto
            {
                Id = 1,
                Name = "Read",
                Description = "Permission to read data",
                Asset = true
            };
            var permission = new Permission { Id = 1, Name = "Read", Description = "Permission to read data" };
            var savedPermission = new Permission { Id = 1, Name = "Read", Description = "Permission to read data" };
            var savedPermissionDto = new PermissionDto { Id = 1, Name = "Read", Description = "Permission to read data" };

            _permissionDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Permission, bool>>>()))
                .ReturnsAsync(false);
            _permissionDataMock.Setup(x => x.Save(It.IsAny<Permission>()))
                .ReturnsAsync(savedPermission);
            _mapperMock.Setup(x => x.Map<Permission>(permissionDto))
                .Returns(permission);
            _mapperMock.Setup(x => x.Map<PermissionDto>(savedPermission))
                .Returns(savedPermissionDto);

            // Act
            var result = await _permissionBusiness.Save(permissionDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedPermissionDto);
        }

        [Fact]
        public async Task GetById_ExistingPermissionId_ReturnsPermissionDto()
        {
            // Arrange
            var permissionId = 1;
            var permission = new Permission { Id = permissionId, Name = "Read" };
            var permissionDto = new PermissionDto { Id = permissionId, Name = "Read" };

            _permissionDataMock.Setup(x => x.GetById(permissionId))
                .ReturnsAsync(permission);
            _mapperMock.Setup(x => x.Map<PermissionDto>(permission))
                .Returns(permissionDto);

            // Act
            var result = await _permissionBusiness.GetById(permissionId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(permissionDto);
        }
    }
}
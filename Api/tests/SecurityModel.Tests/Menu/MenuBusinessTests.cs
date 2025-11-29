using Business.Implementations.Menu;
using Business.Interfaces.Menu;
using Entity.Dtos;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace SecurityModel.Tests.Menu
{
    public class MenuBusinessTests
    {
        private readonly Mock<IMenuData> _menuDataMock;
        private readonly Mock<ILogger<MenuBusiness>> _loggerMock;
        private readonly MenuBusiness _menuBusiness;

        public MenuBusinessTests()
        {
            _menuDataMock = new Mock<IMenuData>();
            _loggerMock = new Mock<ILogger<MenuBusiness>>();

            _menuBusiness = new MenuBusiness(_menuDataMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAll_ValidRequest_ReturnsMenuList()
        {
            // Arrange
            var expectedMenus = new List<GenericDto>
            {
                new GenericDto { Id = 1, Name = "Dashboard" },
                new GenericDto { Id = 2, Name = "Reports" }
            };

            _menuDataMock.Setup(x => x.GetAll())
                .ReturnsAsync(expectedMenus);

            // Act
            var result = await _menuBusiness.GetAll();

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedMenus);
        }

        [Fact]
        public async Task GetById_ExistingMenuId_ReturnsMenuDto()
        {
            // Arrange
            var menuId = 1;
            var menu = new GenericDto { Id = menuId, Name = "Dashboard" };

            _menuDataMock.Setup(x => x.GetById(menuId))
                .ReturnsAsync(menu);

            // Act
            var result = await _menuBusiness.GetById(menuId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(menu);
            result.Name.Should().Be("Dashboard");
        }

        [Fact]
        public async Task GetById_NonExistingMenuId_ReturnsNull()
        {
            // Arrange
            var menuId = 999;

            _menuDataMock.Setup(x => x.GetById(menuId))
                .ReturnsAsync((GenericDto?)null);

            // Act
            var result = await _menuBusiness.GetById(menuId);

            // Assert
            result.Should().BeNull();
        }
    }
}
using AutoMapper;
using Business.Implementations.Security;
using Business.Interfaces.Security;
using Data.Interfaces;
using Data.Interfaces.Parameter;
using Data.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models;
using Entity.Models.Security;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace SecurityModel.Tests.User
{
    public class UserBusinessTests
    {
        private readonly Mock<IUserData> _userDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserBusiness>> _loggerMock;
        private readonly Mock<IClientData> _clientDataMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IRolBusiness> _rolBusinessMock;
        private readonly Mock<IRolParkingUserBusiness> _rolUserBusinessMock;
        private readonly Mock<IJwtAuthenticationService> _jwtServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IPasswordReset> _passwordResetMock;
        private readonly UserBusiness _userBusiness;

        public UserBusinessTests()
        {
            _userDataMock = new Mock<IUserData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UserBusiness>>();
            _clientDataMock = new Mock<IClientData>();
            _emailServiceMock = new Mock<IEmailService>();
            _rolBusinessMock = new Mock<IRolBusiness>();
            _rolUserBusinessMock = new Mock<IRolParkingUserBusiness>();
            _jwtServiceMock = new Mock<IJwtAuthenticationService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _passwordResetMock = new Mock<IPasswordReset>();

            _userBusiness = new UserBusiness(
                _userDataMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _clientDataMock.Object,
                _emailServiceMock.Object,
                _rolBusinessMock.Object,
                _rolUserBusinessMock.Object,
                _jwtServiceMock.Object,
                _passwordHasherMock.Object,
                _passwordResetMock.Object
            );
        }

        #region Save Method Tests

        [Fact]
        public async Task Save_ValidUserDto_ReturnsSavedUserDto()
        {
            // Arrange
            var userDto = new UserDto
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123",
                PersonId = 1,
                Asset = true
            };
            
            var savedUser = new User { Id = 1, Username = "testuser", Email = "test@example.com", PersonId = 1 };
            var savedUserDto = new UserDto { Id = 1, Username = "testuser", Email = "test@example.com", PersonId = 1 };

            _userDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(false);
            _userDataMock.Setup(x => x.Save(It.IsAny<User>()))
                .ReturnsAsync(savedUser);
            _mapperMock.Setup(x => x.Map<User>(userDto))
                .Returns(savedUser);
            _mapperMock.Setup(x => x.Map<UserDto>(savedUser))
                .Returns(savedUserDto);
            _passwordHasherMock.Setup(x => x.HashPassword("password123"))
                .Returns("hashedPassword123");

            // Act
            var result = await _userBusiness.Save(userDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedUserDto);
            _passwordHasherMock.Verify(x => x.HashPassword("password123"), Times.Once);
        }

        [Fact]
        public async Task Save_DuplicateUsername_ThrowsInvalidOperationException()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "existinguser",
                Email = "test@example.com",
                PersonId = 1
            };
            
            _userDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await _userBusiness.Save(userDto));

            exception.Message.Should().Contain("El nombre de usuario ya se encuentra registrado.");
        }

        #endregion
    }
}
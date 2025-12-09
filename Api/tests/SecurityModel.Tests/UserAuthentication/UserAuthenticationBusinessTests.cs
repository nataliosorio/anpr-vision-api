using AutoMapper;
using Business.Implementations.Security.Authentication;
using Business.Interfaces.Security.Authentication;
using Business.Interfaces.Security.PasswordRecovery;
using Data.Interfaces;
using Data.Interfaces.Parameter;
using Data.Interfaces.Security;
using Entity.Dtos.Login;
using Entity.Dtos.Security;
using Entity.Models;
using Entity.Models.Parameter;
using Entity.Models.Security;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace SecurityModel.Tests.UserAuthentication
{
    public class UserAuthenticationBusinessTests
    {
        private readonly Mock<IUserData> _userDataMock;
        private readonly Mock<IUserVerificationBusiness> _verificationBusinessMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserAuthenticationBusiness>> _loggerMock;
        private readonly Mock<IJwtAuthenticationService> _jwtServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IClientData> _clientDataMock;
        private readonly UserAuthenticationBusiness _authenticationBusiness;

        public UserAuthenticationBusinessTests()
        {
            _userDataMock = new Mock<IUserData>();
            _verificationBusinessMock = new Mock<IUserVerificationBusiness>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UserAuthenticationBusiness>>();
            _jwtServiceMock = new Mock<IJwtAuthenticationService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _clientDataMock = new Mock<IClientData>();

            _authenticationBusiness = new UserAuthenticationBusiness(
                _userDataMock.Object,
                _verificationBusinessMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _jwtServiceMock.Object,
                _passwordHasherMock.Object,
                _clientDataMock.Object
            );
        }

        #region LoginWith2FAAsync Tests

        [Fact]
        public async Task LoginWith2FAAsync_ValidCredentials_ReturnsSuccessResponse()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            var user = new User { Id = 1, Username = username, Password = "hashedPassword", Email = "test@example.com" };

            _userDataMock.Setup(x => x.GetUserByUsernameAsync(username))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("hashedPassword", password))
                .Returns(true);
            _verificationBusinessMock.Setup(x => x.GenerateAndSendCodeAsync(user.Id))
                .ReturnsAsync(new ApiResponse<object>(new { userId = user.Id }, true, "Code sent", null));

            // Act
            var result = await _authenticationBusiness.LoginWith2FAAsync(username, password);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _verificationBusinessMock.Verify(x => x.GenerateAndSendCodeAsync(user.Id), Times.Once);
        }

        [Fact]
        public async Task LoginWith2FAAsync_UserNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var username = "nonexistent";
            var password = "password123";

            _userDataMock.Setup(x => x.GetUserByUsernameAsync(username))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authenticationBusiness.LoginWith2FAAsync(username, password);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Usuario no encontrado");
        }

        #endregion
    }
}
using Business.Implementations.Security.PasswordRecovery;
using Business.Interfaces.Security.PasswordRecovery;
using Data.Interfaces;
using Data.Interfaces.Security;
using Entity.Models.Security;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace SecurityModel.Tests.PasswordRecovery
{
    public class PasswordRecoveryBusinessTests
    {
        private readonly Mock<IUserData> _userDataMock;
        private readonly Mock<IPasswordReset> _passwordResetMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ILogger<PasswordRecoveryBusiness>> _loggerMock;
        private readonly PasswordRecoveryBusiness _passwordRecoveryBusiness;

        public PasswordRecoveryBusinessTests()
        {
            _userDataMock = new Mock<IUserData>();
            _passwordResetMock = new Mock<IPasswordReset>();
            _emailServiceMock = new Mock<IEmailService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _loggerMock = new Mock<ILogger<PasswordRecoveryBusiness>>();

            _passwordRecoveryBusiness = new PasswordRecoveryBusiness(
                _userDataMock.Object,
                _passwordResetMock.Object,
                _emailServiceMock.Object,
                _passwordHasherMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task RequestPasswordResetAsync_ValidEmail_ReturnsSuccess()
        {
            // Arrange
            var email = "user@example.com";
            var user = new User { Id = 1, Email = email };

            _userDataMock.Setup(x => x.GetUserByEmailsync(email))
                .ReturnsAsync(user);
            _passwordResetMock.Setup(x => x.CountRequestsSinceAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(2);
            _passwordResetMock.Setup(x => x.Add(It.IsAny<PasswordReset>()))
                .Returns(Task.CompletedTask);
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _passwordRecoveryBusiness.RequestPasswordResetAsync(email);

            // Assert
            _passwordResetMock.Verify(x => x.Add(It.IsAny<PasswordReset>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmailAsync(email, It.IsAny<string>()), Times.Once);
        }
    }
}
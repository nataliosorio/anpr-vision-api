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

namespace SecurityModel.Tests.Client
{
    public class ClientBusinessTests
    {
        private readonly Mock<IClientData> _clientDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ClientBusiness>> _loggerMock;
        private readonly ClientBusiness _clientBusiness;

        public ClientBusinessTests()
        {
            _clientDataMock = new Mock<IClientData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ClientBusiness>>();

            _clientBusiness = new ClientBusiness(_clientDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        #region GetByPersonIdAsync Tests

        [Fact]
        public async Task GetByPersonIdAsync_ExistingPersonId_ReturnsClient()
        {
            // Arrange
            var personId = 1;
            var client = new Client { Id = 1, PersonId = personId, Asset = true };

            _clientDataMock.Setup(x => x.GetByPersonIdAsync(personId))
                .ReturnsAsync(client);

            // Act
            var result = await _clientBusiness.GetByPersonIdAsync(personId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(client);
            result.PersonId.Should().Be(personId);
        }

        [Fact]
        public async Task GetByPersonIdAsync_NonExistingPersonId_ReturnsNull()
        {
            // Arrange
            var personId = 999;

            _clientDataMock.Setup(x => x.GetByPersonIdAsync(personId))
                .ReturnsAsync((Client?)null);

            // Act
            var result = await _clientBusiness.GetByPersonIdAsync(personId);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region Save Tests

        [Fact]
        public async Task Save_ValidClientDto_ReturnsSavedClientDto()
        {
            // Arrange
            var clientDto = new ClientDto
            {
                Id = 1,
                PersonId = 1,
                Asset = true
            };
            var client = new Client { Id = 1, PersonId = 1 };
            var savedClient = new Client { Id = 1, PersonId = 1 };
            var savedClientDto = new ClientDto { Id = 1, PersonId = 1 };

            _clientDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                .ReturnsAsync(false);
            _clientDataMock.Setup(x => x.Save(It.IsAny<Client>()))
                .ReturnsAsync(savedClient);
            _mapperMock.Setup(x => x.Map<Client>(clientDto))
                .Returns(client);
            _mapperMock.Setup(x => x.Map<ClientDto>(savedClient))
                .Returns(savedClientDto);

            // Act
            var result = await _clientBusiness.Save(clientDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedClientDto);
        }

        #endregion
    }
}
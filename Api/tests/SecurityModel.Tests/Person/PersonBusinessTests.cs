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

namespace SecurityModel.Tests.Person
{
    public class PersonBusinessTests
    {
        private readonly Mock<IPersonData> _personDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PersonBusiness>> _loggerMock;
        private readonly PersonBusiness _personBusiness;

        public PersonBusinessTests()
        {
            _personDataMock = new Mock<IPersonData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PersonBusiness>>();

            _personBusiness = new PersonBusiness(_personDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        #region Save Tests

        [Fact]
        public async Task Save_ValidPersonDto_ReturnsSavedPersonDto()
        {
            // Arrange
            var personDto = new PersonDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DocumentNumber = "123456789",
                Email = "john.doe@example.com",
                Asset = true
            };
            var person = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
            var savedPerson = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
            var savedPersonDto = new PersonDto { Id = 1, FirstName = "John", LastName = "Doe" };

            _personDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(false);
            _personDataMock.Setup(x => x.Save(It.IsAny<Person>()))
                .ReturnsAsync(savedPerson);
            _mapperMock.Setup(x => x.Map<Person>(personDto))
                .Returns(person);
            _mapperMock.Setup(x => x.Map<PersonDto>(savedPerson))
                .Returns(savedPersonDto);

            // Act
            var result = await _personBusiness.Save(personDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedPersonDto);
        }

        [Fact]
        public async Task Save_DuplicateDocumentNumber_ThrowsInvalidOperationException()
        {
            // Arrange
            var personDto = new PersonDto
            {
                DocumentNumber = "123456789",
                FirstName = "John",
                LastName = "Doe"
            };

            _personDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await _personBusiness.Save(personDto));

            exception.Message.Should().Contain("ya se encuentra registrado");
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_ExistingPersonId_ReturnsPersonDto()
        {
            // Arrange
            var personId = 1;
            var person = new Person { Id = personId, FirstName = "John", LastName = "Doe" };
            var personDto = new PersonDto { Id = personId, FirstName = "John", LastName = "Doe" };

            _personDataMock.Setup(x => x.GetById(personId))
                .ReturnsAsync(person);
            _mapperMock.Setup(x => x.Map<PersonDto>(person))
                .Returns(personDto);

            // Act
            var result = await _personBusiness.GetById(personId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(personDto);
        }

        #endregion
    }
}
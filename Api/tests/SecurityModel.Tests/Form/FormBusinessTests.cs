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

namespace SecurityModel.Tests.Form
{
    public class FormBusinessTests
    {
        private readonly Mock<IFormData> _formDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<FormBusiness>> _loggerMock;
        private readonly FormBusiness _formBusiness;

        public FormBusinessTests()
        {
            _formDataMock = new Mock<IFormData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<FormBusiness>>();

            _formBusiness = new FormBusiness(_formDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Save_ValidFormDto_ReturnsSavedFormDto()
        {
            // Arrange
            var formDto = new FormDto
            {
                Id = 1,
                Name = "User Management",
                Description = "Form for user management",
                Asset = true
            };
            var form = new Form { Id = 1, Name = "User Management", Description = "Form for user management" };
            var savedForm = new Form { Id = 1, Name = "User Management", Description = "Form for user management" };
            var savedFormDto = new FormDto { Id = 1, Name = "User Management", Description = "Form for user management" };

            _formDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Form, bool>>>()))
                .ReturnsAsync(false);
            _formDataMock.Setup(x => x.Save(It.IsAny<Form>()))
                .ReturnsAsync(savedForm);
            _mapperMock.Setup(x => x.Map<Form>(formDto))
                .Returns(form);
            _mapperMock.Setup(x => x.Map<FormDto>(savedForm))
                .Returns(savedFormDto);

            // Act
            var result = await _formBusiness.Save(formDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedFormDto);
            result.Name.Should().Be("User Management");
        }

        [Fact]
        public async Task GetById_ExistingFormId_ReturnsFormDto()
        {
            // Arrange
            var formId = 1;
            var form = new Form { Id = formId, Name = "Test Form" };
            var formDto = new FormDto { Id = formId, Name = "Test Form" };

            _formDataMock.Setup(x => x.GetById(formId))
                .ReturnsAsync(form);
            _mapperMock.Setup(x => x.Map<FormDto>(form))
                .Returns(formDto);

            // Act
            var result = await _formBusiness.GetById(formId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(formDto);
        }
    }
}
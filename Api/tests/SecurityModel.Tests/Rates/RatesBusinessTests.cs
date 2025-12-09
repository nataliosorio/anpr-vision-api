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

namespace SecurityModel.Tests.Rates
{
    public class RatesBusinessTests
    {
        private readonly Mock<IRatesData> _ratesDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<RatesBusiness>> _loggerMock;
        private readonly RatesBusiness _ratesBusiness;

        public RatesBusinessTests()
        {
            _ratesDataMock = new Mock<IRatesData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RatesBusiness>>();

            _ratesBusiness = new RatesBusiness(_ratesDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Save_ValidRatesDto_ReturnsSavedRatesDto()
        {
            // Arrange
            var ratesDto = new RatesDto
            {
                Id = 1,
                Name = "Hourly Rate",
                Price = 5.00m,
                ParkingId = 1,
                RatesTypeId = 1,
                Asset = true
            };
            var rates = new Rates { Id = 1, Name = "Hourly Rate", Price = 5.00m, ParkingId = 1, RatesTypeId = 1 };
            var savedRates = new Rates { Id = 1, Name = "Hourly Rate", Price = 5.00m, ParkingId = 1, RatesTypeId = 1 };
            var savedRatesDto = new RatesDto { Id = 1, Name = "Hourly Rate", Price = 5.00m, ParkingId = 1, RatesTypeId = 1 };

            _ratesDataMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Rates, bool>>>()))
                .ReturnsAsync(false);
            _ratesDataMock.Setup(x => x.Save(It.IsAny<Rates>()))
                .ReturnsAsync(savedRates);
            _mapperMock.Setup(x => x.Map<Rates>(ratesDto))
                .Returns(rates);
            _mapperMock.Setup(x => x.Map<RatesDto>(savedRates))
                .Returns(savedRatesDto);

            // Act
            var result = await _ratesBusiness.Save(ratesDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(savedRatesDto);
            result.Name.Should().Be("Hourly Rate");
            result.Price.Should().Be(5.00m);
        }

        [Fact]
        public async Task GetById_ExistingRatesId_ReturnsRatesDto()
        {
            // Arrange
            var ratesId = 1;
            var rates = new Rates { Id = ratesId, Name = "Test Rate", Price = 10.00m };
            var ratesDto = new RatesDto { Id = ratesId, Name = "Test Rate", Price = 10.00m };

            _ratesDataMock.Setup(x => x.GetById(ratesId))
                .ReturnsAsync(rates);
            _mapperMock.Setup(x => x.Map<RatesDto>(rates))
                .Returns(ratesDto);

            // Act
            var result = await _ratesBusiness.GetById(ratesId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(ratesDto);
        }
    }
}
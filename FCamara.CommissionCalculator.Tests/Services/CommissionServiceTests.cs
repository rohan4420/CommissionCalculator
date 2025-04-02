using FCamara.CommissionCalculator.Domain.Constants;
using FCamara.CommissionCalculator.Domain.Models;
using FCamara.CommissionCalculator.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace FCamara.CommissionCalculator.Tests.Services
{
    public class CommissionServiceTests
    {
        private readonly Mock<ILogger<CommissionService>> _loggerMock;
        private readonly CommissionService _service;

        public CommissionServiceTests()
        {
            // Setup mock logger
            _loggerMock = new Mock<ILogger<CommissionService>>();
            
            // Initialize the service with the mock logger
            _service = new CommissionService(_loggerMock.Object);
        }

        [Fact]
        public void CalculateCommission_ValidRequest_ReturnsCorrectCalculation()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = 10,
                ForeignSalesCount = 5
            };

            // Expected values
            decimal expectedFCamaraCommission = Math.Round(
                (request.LocalSalesCount * request.AverageSaleAmount * CommissionRates.FCamaraLocalRate) +
                (request.ForeignSalesCount * request.AverageSaleAmount * CommissionRates.FCamaraForeignRate), 2);

            decimal expectedCompetitorCommission = Math.Round(
                (request.LocalSalesCount * request.AverageSaleAmount * CommissionRates.CompetitorLocalRate) +
                (request.ForeignSalesCount * request.AverageSaleAmount * CommissionRates.CompetitorForeignRate), 2);

            // Act
            var result = _service.CalculateCommission(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFCamaraCommission, result.FCamaraCommissionAmount);
            Assert.Equal(expectedCompetitorCommission, result.CompetitorCommissionAmount);
        }

        [Fact]
        public void CalculateCommission_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            CommissionCalculationRequest request = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _service.CalculateCommission(request));
            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData(-1)]
        public void CalculateCommission_InvalidAverageSaleAmount_ThrowsArgumentException(decimal averageSaleAmount)
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = averageSaleAmount,
                LocalSalesCount = 10,
                ForeignSalesCount = 5
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _service.CalculateCommission(request));
            Assert.Contains("Average sale amount must be greater than zero", exception.Message);
        }

        [Fact]
        public void CalculateCommission_NegativeLocalSalesCount_ThrowsArgumentException()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = -1,
                ForeignSalesCount = 5
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _service.CalculateCommission(request));
            Assert.Contains("Local sales count be cannot be negative", exception.Message);
        }

        [Fact]
        public void CalculateCommission_NegativeForeignSalesCount_ThrowsArgumentException()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = 5,
                ForeignSalesCount = -1
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _service.CalculateCommission(request));
            Assert.Contains("Foreign sales count must be non-negative", exception.Message);
        }

        [Fact]
        public void CalculateCommission_ZeroTotalSalesCount_ThrowsArgumentException()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = 0,
                ForeignSalesCount = 0
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _service.CalculateCommission(request));
            Assert.Contains("At least one sale is required", exception.Message);
        }

        [Fact]
        public void CalculateCommission_OnlyLocalSales_CalculatesCorrectly()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = 10,
                ForeignSalesCount = 0
            };

            // Expected values
            decimal expectedFCamaraCommission = Math.Round(
                request.LocalSalesCount * request.AverageSaleAmount * CommissionRates.FCamaraLocalRate, 2);

            decimal expectedCompetitorCommission = Math.Round(
                request.LocalSalesCount * request.AverageSaleAmount * CommissionRates.CompetitorLocalRate, 2);

            // Act
            var result = _service.CalculateCommission(request);

            // Assert
            Assert.Equal(expectedFCamaraCommission, result.FCamaraCommissionAmount);
            Assert.Equal(expectedCompetitorCommission, result.CompetitorCommissionAmount);
        }

        [Fact]
        public void CalculateCommission_OnlyForeignSales_CalculatesCorrectly()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = 0,
                ForeignSalesCount = 10
            };

            // Expected values
            decimal expectedFCamaraCommission = Math.Round(
                request.ForeignSalesCount * request.AverageSaleAmount * CommissionRates.FCamaraForeignRate, 2);

            decimal expectedCompetitorCommission = Math.Round(
                request.ForeignSalesCount * request.AverageSaleAmount * CommissionRates.CompetitorForeignRate, 2);

            // Act
            var result = _service.CalculateCommission(request);

            // Assert
            Assert.Equal(expectedFCamaraCommission, result.FCamaraCommissionAmount);
            Assert.Equal(expectedCompetitorCommission, result.CompetitorCommissionAmount);
        }

        [Fact]
        public void CalculateCommission_VerifyLoggerIsCalled()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = 10,
                ForeignSalesCount = 5
            };

            // Act
            _service.CalculateCommission(request);

            // Assert
            // Verify that LogInformation was called with the expected message
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Calculating commission for request")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void CalculateCommission_DecimalPrecision_RoundsCorrectly()
        {
            // Arrange - Using values that will create decimal places
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 100.33m,
                LocalSalesCount = 3,
                ForeignSalesCount = 2
            };

            // Expected values with explicit rounding to 2 decimal places
            decimal expectedFCamaraCommission = Math.Round(
                (request.LocalSalesCount * request.AverageSaleAmount * CommissionRates.FCamaraLocalRate) +
                (request.ForeignSalesCount * request.AverageSaleAmount * CommissionRates.FCamaraForeignRate), 2);

            decimal expectedCompetitorCommission = Math.Round(
                (request.LocalSalesCount * request.AverageSaleAmount * CommissionRates.CompetitorLocalRate) +
                (request.ForeignSalesCount * request.AverageSaleAmount * CommissionRates.CompetitorForeignRate), 2);

            // Act
            var result = _service.CalculateCommission(request);

            // Assert
            Assert.Equal(expectedFCamaraCommission, result.FCamaraCommissionAmount);
            Assert.Equal(expectedCompetitorCommission, result.CompetitorCommissionAmount);
            
            // Verify results have exactly 2 decimal places
            Assert.Equal(2, BitConverter.GetBytes(decimal.GetBits(result.FCamaraCommissionAmount)[3])[2]);
            Assert.Equal(2, BitConverter.GetBytes(decimal.GetBits(result.CompetitorCommissionAmount)[3])[2]);
        }
    }
}
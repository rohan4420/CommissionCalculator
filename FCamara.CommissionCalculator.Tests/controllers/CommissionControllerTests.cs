using FCamara.CommissionCalculator.Controllers;
using FCamara.CommissionCalculator.Domain.Models;
using FCamara.CommissionCalculator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace FCamara.CommissionCalculator.Tests.Controllers
{
    public class CommissionControllerTests
    {
        private readonly Mock<ICommissionService> _commissionServiceMock;
        private readonly Mock<ILogger<CommissionController>> _loggerMock;
        private readonly CommissionController _controller;

        public CommissionControllerTests()
        {
            // Setup mocks
            _commissionServiceMock = new Mock<ICommissionService>();
            _loggerMock = new Mock<ILogger<CommissionController>>();
            
            // Initialize the controller with mocks
            _controller = new CommissionController(_commissionServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Calculate_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = 10,
                ForeignSalesCount = 5
            };

            var expectedResponse = new CommissionCalculationResponse
            {
                FCamaraCommissionAmount = 1234.56m,
                CompetitorCommissionAmount = 789.10m
            };

            _commissionServiceMock.Setup(x => x.CalculateCommission(It.IsAny<CommissionCalculationRequest>()))
                .Returns(expectedResponse);

            // Act
            var result = _controller.Calculate(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CommissionCalculationResponse>(okResult.Value);
            Assert.Equal(expectedResponse.FCamaraCommissionAmount, returnValue.FCamaraCommissionAmount);
            Assert.Equal(expectedResponse.CompetitorCommissionAmount, returnValue.CompetitorCommissionAmount);
        }

        [Fact]
        public void Calculate_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new CommissionCalculationRequest(); // Invalid request
            _controller.ModelState.AddModelError("AverageSaleAmount", "The AverageSaleAmount field is required");

            // Act
            var result = _controller.Calculate(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Calculate_ArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = 10,
                ForeignSalesCount = 0
            };

            var exceptionMessage = "Average sale amount must be greater than zero";
            _commissionServiceMock.Setup(x => x.CalculateCommission(It.IsAny<CommissionCalculationRequest>()))
                .Throws(new ArgumentException(exceptionMessage));

            // Act
            var result = _controller.Calculate(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }

        [Fact]
        public void Calculate_GenericException_Returns500Error()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = 10,
                ForeignSalesCount = 5
            };

            _commissionServiceMock.Setup(x => x.CalculateCommission(It.IsAny<CommissionCalculationRequest>()))
                .Throws(new Exception("Unexpected error"));

            // Act
            var result = _controller.Calculate(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred", statusCodeResult.Value);
            
            // Verify logger was called
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Calculate_VerifyServiceIsCalled()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                AverageSaleAmount = 1000m,
                LocalSalesCount = 10,
                ForeignSalesCount = 5
            };

            _commissionServiceMock.Setup(x => x.CalculateCommission(It.IsAny<CommissionCalculationRequest>()))
                .Returns(new CommissionCalculationResponse());

            // Act
            _controller.Calculate(request);

            // Assert
            _commissionServiceMock.Verify(x => x.CalculateCommission(request), Times.Once);
        }
    }
}
// api/Services/CommissionService.cs
using FCamara.CommissionCalculator.Domain.Constants;
using FCamara.CommissionCalculator.Domain.Models;
using FCamara.CommissionCalculator.Services.Interfaces;
using Microsoft.Extensions.Logging;
using FCamara.CommissionCalculator.Validators;
using FCamara.CommissionCalculator.Calculations;

namespace FCamara.CommissionCalculator.Services
{
    public class CommissionService : ICommissionService
    {
        private readonly ILogger<CommissionService> _logger;
        private readonly CommissionCalculationRequestValidator _validator;
        private readonly ComputeCalculator _calculator;
        
        public CommissionService(ILogger<CommissionService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validator = new CommissionCalculationRequestValidator();
            _calculator = new ComputeCalculator();
        }

        public CommissionCalculationResponse CalculateCommission(CommissionCalculationRequest request)
        {
            _logger.LogInformation("Calculating commission for request");
            _validator.ValidateRequest(request);                 
        
            // Calculate FCamara commission
            decimal fCamaraCommission = _calculator.CalculateFCamaraCommission(request);
            // Calculate competitor commission
            decimal competitorCommission = _calculator.CalculateCompetitorCommission(request);
            // Create response
            var response = new CommissionCalculationResponse
            {
                FCamaraCommissionAmount = Math.Round(fCamaraCommission, 2),
                CompetitorCommissionAmount = Math.Round(competitorCommission, 2)
            };

            return response;
        }
        // public decimal CalculateFCamaraCommission(CommissionCalculationRequest request)
        // {
        //     decimal localCommission = request.LocalSalesCount * request.AverageSaleAmount * CommissionRates.FCamaraLocalRate;
        //     decimal foreignCommission = request.ForeignSalesCount * request.AverageSaleAmount * CommissionRates.FCamaraForeignRate;
        //     return localCommission + foreignCommission;
        // }
        
        // public decimal CalculateCompetitorCommission(CommissionCalculationRequest request)
        // {
        //     decimal localCommission = request.LocalSalesCount * request.AverageSaleAmount * CommissionRates.CompetitorLocalRate;
        //     decimal foreignCommission = request.ForeignSalesCount * request.AverageSaleAmount * CommissionRates.CompetitorForeignRate;
        //     return localCommission + foreignCommission;
        // }
    }
}
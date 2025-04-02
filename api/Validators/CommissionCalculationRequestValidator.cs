using FCamara.CommissionCalculator.Domain.Models;
using System;

namespace FCamara.CommissionCalculator.Validators
{
    public class CommissionCalculationRequestValidator
    {
        public void ValidateRequest(CommissionCalculationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
                
            if (request.ForeignSalesCount < 0)
                throw new ArgumentException("Foreign sales count must be non-negative", nameof(request));
                
            if (request.LocalSalesCount < 0)
                throw new ArgumentException("Local sales count be cannot be negative", nameof(request));
                
            if (request.AverageSaleAmount <= 0)
                throw new ArgumentException("Average sale amount must be greater than zero", nameof(request));
                
            if (request.LocalSalesCount == 0 && request.ForeignSalesCount == 0)
                throw new ArgumentException("At least one sale is required", nameof(request));
        }
    }
}
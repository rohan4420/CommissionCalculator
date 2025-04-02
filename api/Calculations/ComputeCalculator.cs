using FCamara.CommissionCalculator.Domain.Constants;
using FCamara.CommissionCalculator.Domain.Models;
using System;

namespace FCamara.CommissionCalculator.Calculations
{
    public class ComputeCalculator
    {
        public decimal CalculateFCamaraCommission(CommissionCalculationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
                
            decimal localCommission = request.LocalSalesCount * request.AverageSaleAmount * CommissionRates.FCamaraLocalRate;
            decimal foreignCommission = request.ForeignSalesCount * request.AverageSaleAmount * CommissionRates.FCamaraForeignRate;
            return localCommission + foreignCommission;
        }
        
        public decimal CalculateCompetitorCommission(CommissionCalculationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
                
            decimal localCommission = request.LocalSalesCount * request.AverageSaleAmount * CommissionRates.CompetitorLocalRate;
            decimal foreignCommission = request.ForeignSalesCount * request.AverageSaleAmount * CommissionRates.CompetitorForeignRate;
            return localCommission + foreignCommission;
        }
    }
}
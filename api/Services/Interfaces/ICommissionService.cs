// api/Services/Interfaces/ICommissionService.cs
using FCamara.CommissionCalculator.Domain.Models;

namespace FCamara.CommissionCalculator.Services.Interfaces
{
    public interface ICommissionService
    {
        CommissionCalculationResponse CalculateCommission(CommissionCalculationRequest request);
    }
}
using System.ComponentModel.DataAnnotations;

namespace FCamara.CommissionCalculator.Domain.Models
{
   public class CommissionCalculationRequest
{
    [Range(0, int.MaxValue, ErrorMessage = "Local sales count be cannot be negative")]
    public int LocalSalesCount { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Foreign sales count cannot be negative")]
    public int ForeignSalesCount { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Average sale amount must be greater than zero")]
    public decimal AverageSaleAmount { get; set; }
}

}



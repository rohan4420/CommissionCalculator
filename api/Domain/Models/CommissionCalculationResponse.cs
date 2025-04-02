namespace FCamara.CommissionCalculator.Domain.Models
{
    public class CommissionCalculationResponse
    {
        public decimal FCamaraCommissionAmount { get; set; }
        public decimal CompetitorCommissionAmount { get; set; }
    }
}
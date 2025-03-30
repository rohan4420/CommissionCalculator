using Microsoft.AspNetCore.Mvc;

namespace FCamara.CommissionCalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommissionController : ControllerBase
    {
        private readonly ILogger<CommissionController> _logger;
        
        // Commission rates as constants for easy maintenance
        private const decimal FCamaraLocalRate = 0.20m;
        private const decimal FCamaraForeignRate = 0.35m;
        private const decimal CompetitorLocalRate = 0.02m;
        private const decimal CompetitorForeignRate = 0.0755m;
        
        // Volume bonus thresholds and rates
        private const int VolumeBonusThreshold = 50;
        private const decimal VolumeBonusRate = 0.01m;
        
        // High value bonus thresholds and rates
        private const decimal HighValueBonusThreshold = 1000m;
        private const decimal HighValueBonusRate = 0.02m;

        public CommissionController(ILogger<CommissionController> logger)
        {
            _logger = logger;
        }

        [ProducesResponseType(typeof(CommissionCalculationResponse), 200)]
        [HttpPost]
        public IActionResult Calculate([FromBody] CommissionCalculationRequest calculationRequest)
        {
            _logger.LogInformation("Calculating commission for request: {@Request}", calculationRequest);

            // Input validation
            if (calculationRequest == null || 
                calculationRequest.AverageSaleAmount <= 0 ||
                calculationRequest.LocalSalesCount < 0 ||
                calculationRequest.ForeignSalesCount < 0)
            {
                _logger.LogWarning("Invalid input data received");
                return BadRequest("Invalid input data");
            }

            // Calculate total sales
            int totalSales = calculationRequest.LocalSalesCount + calculationRequest.ForeignSalesCount;
            
            if (totalSales <= 0)
            {
                _logger.LogWarning("At least one sale is required to calculate commission");
                return BadRequest("At least one sale is required to calculate commission");
            }

            // Calculate total sale amount
            decimal totalSaleAmount = totalSales * calculationRequest.AverageSaleAmount;

            // Calculate FCamara base commission
            decimal fCamaraCommission = 
                (calculationRequest.LocalSalesCount * calculationRequest.AverageSaleAmount * FCamaraLocalRate) +
                (calculationRequest.ForeignSalesCount * calculationRequest.AverageSaleAmount * FCamaraForeignRate);
            
            // Apply volume bonus 
            if (totalSales > VolumeBonusThreshold)
            {
                decimal volumeBonus = totalSaleAmount * VolumeBonusRate;
                fCamaraCommission += volumeBonus;
                _logger.LogInformation("Applied volume bonus of {VolumeBonus}", volumeBonus);
            }
            
            // Apply high value bonus
            if (calculationRequest.AverageSaleAmount > HighValueBonusThreshold)
            {
                decimal highValueBonus = totalSaleAmount * HighValueBonusRate;
                fCamaraCommission += highValueBonus;
                _logger.LogInformation("Applied high value bonus of {HighValueBonus}", highValueBonus);
            }

            // Calculate competitor commission
            decimal competitorCommission = 
                (calculationRequest.LocalSalesCount * calculationRequest.AverageSaleAmount * CompetitorLocalRate) +
                (calculationRequest.ForeignSalesCount * calculationRequest.AverageSaleAmount * CompetitorForeignRate);

            // Create response with rounded values
            var response = new CommissionCalculationResponse
            {
                FCamaraCommissionAmount = Math.Round(fCamaraCommission, 2),
                CompetitorCommissionAmount = Math.Round(competitorCommission, 2)
            };

            _logger.LogInformation("Commission calculation result: {@Response}", response);

            return Ok(response);
        }
    }

    public class CommissionCalculationRequest
    {
        public int LocalSalesCount { get; set; }
        public int ForeignSalesCount { get; set; }
        public decimal AverageSaleAmount { get; set; }
    }

    public class CommissionCalculationResponse
    {
        public decimal FCamaraCommissionAmount { get; set; }

        public decimal CompetitorCommissionAmount { get; set; }
    }
}

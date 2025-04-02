// api/Controllers/CommissionController.cs
using FCamara.CommissionCalculator.Domain.Models;
using FCamara.CommissionCalculator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FCamara.CommissionCalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommissionController : ControllerBase
    {
        private readonly ICommissionService _commissionService;
        private readonly ILogger<CommissionController> _logger;

        public CommissionController(ICommissionService commissionService, ILogger<CommissionController> logger)
        {
            _commissionService = commissionService;
            _logger = logger;
        }

[HttpPost]
[ProducesResponseType(typeof(CommissionCalculationResponse), 200)]
public IActionResult Calculate([FromBody] CommissionCalculationRequest request)
{
    if (!ModelState.IsValid)
    {
        // Return BadRequest with error messages
        var errorMessages = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
            
        return BadRequest(new { message = errorMessages });
    }

    try
    {
        var response = _commissionService.CalculateCommission(request);
        return Ok(response);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error calculating commission");
        return StatusCode(500, "An error occurred");
    }
    }

    }
}
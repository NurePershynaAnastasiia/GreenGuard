using Microsoft.AspNetCore.Mvc;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Watering;

namespace GreenGuard.Controllers.FeaturesControllers
{
    // api/Fertilizers
    [ApiController]
    [Route("api/[controller]")]
    public class WateringController : ControllerBase
    {
        private readonly ILogger<WateringController> _logger;

        public WateringController(ILogger<WateringController> logger, WateringService wateringService)
        {
            _logger = logger;
            _wateringService = wateringService;
        }
        private readonly WateringService _wateringService;

        // GET: api/Watering/calculate-watering
        [HttpGet("calculate-watering")]
        public async Task<IActionResult> CalculateNextWatering(int plantId)
        {
            try
            {
                var nextWateringDate = await _wateringService.CalculateNextWateringAsync(plantId);
                return Ok(nextWateringDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during calculating watering schedule");
                return StatusCode(500);
            }

        }

    }
}

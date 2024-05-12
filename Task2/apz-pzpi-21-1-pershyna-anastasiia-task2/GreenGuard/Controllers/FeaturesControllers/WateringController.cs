using Microsoft.AspNetCore.Mvc;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Watering;
using GreenGuard.Helpers;
using Microsoft.AspNetCore.Authorization;

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

        /// <summary>
        /// Calculate the date of the next watering for a specific plant.
        /// </summary>
        /// <param name="plantId">The ID of the plant for which to calculate the next watering date.</param>
        /// <returns>
        /// If the calculation is successful, it will return the date of the next watering.
        /// If there is an error during calculation, it will return a 500 Internal Server Error.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("{plantId}")]
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

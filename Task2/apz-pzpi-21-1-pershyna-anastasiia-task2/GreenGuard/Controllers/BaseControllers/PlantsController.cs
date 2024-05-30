using Microsoft.AspNetCore.Mvc;
using GreenGuard.Models.Plant;
using GreenGuard.Helpers;
using Microsoft.AspNetCore.Authorization;
using GreenGuard.Services;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/Plants
    [ApiController]
    [Route("api/[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly ILogger<PlantsController> _logger;
        private readonly PlantService _plantService;
        private readonly PlantStatusService _plantStatusService;

        public PlantsController(ILogger<PlantsController> logger, PlantService plantService, PlantStatusService plantStatusService)
        {
            _logger = logger;
            _plantService = plantService;
            _plantStatusService = plantStatusService;
        }

        /// <summary>
        /// Get a list of all plants.
        /// </summary>
        /// <returns>
        /// If the operation is successful, it will return a list of PlantTypeDto.
        /// If there is a bad request, it will return an ErrorDto.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("plants")]
        public async Task<IActionResult> GetPlants()
        {
            try
            {
                var plants = await _plantService.GetPlants();
                return Ok(plants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting plants");
                return StatusCode(500, "An error occurred while getting plants");
            }
        }

        /// <summary>
        /// Add a new plant.
        /// </summary>
        /// <param name="newPlantTypeId">The ID of the new plant type.</param>
        /// <param name="newPlantLocation">The location of the new plant.</param>
        /// <returns>
        /// If the operation is successful, it will return a success message.
        /// If the provided model is invalid, it will return a 400 Bad Request response.
        /// If an error occurs, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpPost("add")]
        public async Task<ActionResult> AddPlant(AddPlant model)
        {
            try
            {
                await _plantService.AddNewPlant(model);
                await _plantStatusService.UpdatePlantStatus();
                return Ok("Plant was successfully added");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new plant");
                return StatusCode(500, "An error occurred while adding a new plant");
            }
        }

        /// <summary>
        /// Update the details of a plant.
        /// </summary>
        /// <param name="id">The ID of the plant to update.</param>
        /// <param name="model">The updated details of the plant.</param>
        /// <returns>
        /// If the operation is successful, it will return a success message.
        /// If there is no plant with the provided ID, it will return a 404 Not Found response.
        /// If an error occurs, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePlant(int id, UpdatePlant model)
        {
            try
            {
                await _plantService.UpdatePlant(id, model);
                await _plantStatusService.UpdatePlantStatus();
                return Ok("Plant details updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating plant");
                return StatusCode(500, "An error occurred while updating plant");
            }
        }

        /// <summary>
        /// Delete a plant by its ID.
        /// </summary>
        /// <param name="id">The ID of the plant to delete.</param>
        /// <returns>
        /// If the operation is successful, it will return a success message.
        /// If there is no plant with the provided ID, it will return a 400 Bad Request response.
        /// If an error occurs, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePlant(int id)
        {
            try
            {
                await _plantService.DeletePlant(id);
                return Ok($"Plant with ID {id} was successfully deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting plant");
                return StatusCode(500, "An error occurred while deleting plant");
            }
        }
    }
}

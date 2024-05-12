using Microsoft.AspNetCore.Mvc;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Plant;
using GreenGuard.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/Plants
    [ApiController]
    [Route("api/[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<PlantsController> _logger;

        public PlantsController(GreenGuardDbContext context, ILogger<PlantsController> logger)
        {
            _context = context;
            _logger = logger;
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
                var plants = _context.Plant.Select(data => new PlantFull
                {
                    PlantId = data.PlantId,
                    PlantTypeId = data.PlantTypeId,
                    PlantLocation = data.PlantLocation,
                    Humidity = data.Humidity,
                    Temp = data.Temp,
                    Light = data.Light,
                    AdditionalInfo = data.AdditionalInfo,
                    PlantState = data.PlantState,
                    Pests = _context.Pest_in_Plant
                    .Where(pip => pip.PlantId == data.PlantId)
                    .Select(pip => _context.Pest.FirstOrDefault(p => p.PestId == pip.PestId).PestName)
                    .ToList()
                }).ToList();
                return Ok(plants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all plants loading");
                return StatusCode(500, ex.Message);
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
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("add")]
        public async Task<ActionResult> AddNewPlant(AddPlant model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);

                }

                var newPlant = new PlantDto
                {
                    PlantTypeId = model.PlantTypeId,
                    PlantLocation = model.PlantLocation,
                    Humidity = model.Humidity,
                    Light = model.Light,
                    Temp = model.Temp,
                    AdditionalInfo = model.AdditionalInfo,
                    PlantState = model.PlantState
                };

                _context.Add(newPlant);
                await _context.SaveChangesAsync();
                return Ok("Plant was succesfully added");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during adding new plant");
                return StatusCode(500, ex.Message);
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
        [Authorize(Roles = Roles.Administrator)]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePlant(int id, UpdatePlant model)
        {
            try
            {
                var plant = await _context.Plant.FindAsync(id);
                if (plant == null)
                {
                    return NotFound("There is no plant with the provided ID.");
                }

                plant.PlantLocation = model.PlantLocation;
                plant.Humidity = model.Humidity;
                plant.Light = model.Light;
                plant.Temp = model.Temp;
                plant.AdditionalInfo = model.AdditionalInfo;
                plant.PlantState = model.PlantState;

                _context.Update(plant);
                await _context.SaveChangesAsync();

                return Ok("Plant details updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during editing plant");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update the state of a plant by its ID.
        /// </summary>
        /// <param name="id">The ID of the plant to update.</param>
        /// <param name="model">The updated state of the plant.</param>
        /// <returns>
        /// If the operation is successful, it will return a success message.
        /// If there is no plant with the provided ID, it will return a 404 Not Found response.
        /// If an error occurs, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpPut("update-state/{id}")]
        public async Task<IActionResult> UpdatePlantState(int id, UpdatePlantState model)
        {
            try
            {
                var plant = await _context.Plant.FindAsync(id);
                if (plant == null)
                {
                    return NotFound("There is no pant with the provided ID");
                }

                plant.PlantState = model.PlantState;
                _context.Update(plant);
                await _context.SaveChangesAsync();

                return Ok($"Plant state {plant.PlantState} updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during updating state of plant");
                return StatusCode(500, ex.Message);
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
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePlant(int id)
        {
            try
            {
                var plant = await _context.Plant.FindAsync(id);
                if (plant == null)
                {
                    return BadRequest(ModelState);
                }
                _context.Plant.Remove(plant);
                await _context.SaveChangesAsync();

                return Ok($"Plant with location: {plant.PlantLocation} was successfully deleted");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during deleting plant");
                return StatusCode(500, ex.Message);

            }

        }
    }
}

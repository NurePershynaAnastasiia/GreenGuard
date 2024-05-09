using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Pest;
using GreenGuard.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/Pests
    [ApiController]
    [Route("api/[controller]")]

    public class PestsController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<PestsController> _logger;

        public PestsController(GreenGuardDbContext context, ILogger<PestsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of all pests.
        /// </summary>
        /// <returns>
        /// If the operation is successful, it will return an ICollection of PestDto.
        /// If there is a bad request, it will return an ErrorDto.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("pests")]
        public async Task<IActionResult> GetPests()
        {
            try
            {
                var pests = _context.Pest.Select(data => new PestDto
                {
                    PestId = data.PestId,
                    PestName = data.PestName,
                    PestDescription = data.PestDescription
                }).ToList();
                return Ok(pests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all pests loading");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get a pest by its ID.
        /// </summary>
        /// <param name="pestId">The ID of the pest to retrieve.</param>
        /// <returns>
        /// If the pest with the specified ID is found, it will return the PestDto.
        /// If the pest with the specified ID is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("{pestId}")]
        public async Task<IActionResult> GetPestById(int pestId)
        {
            try
            {
                var pest = await _context.Pest.FindAsync(pestId);
                if (pest == null)
                {
                    return NotFound($"Pest with ID {pestId} not found");
                }

                var pestDto = new AddPest
                {
                    PestName = pest.PestName,
                    PestDescription = pest.PestDescription
                };

                return Ok(pestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving pest by ID");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add a new pest.
        /// </summary>
        /// <param name="model">The data to add a new pest.</param>
        /// <returns>
        /// If the operation is successful, it will return a message confirming the addition.
        /// If there is a bad request, it will return an ErrorDto.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("add")]
        public async Task<IActionResult> AddPest(AddPest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _context.Pest.AnyAsync(data => data.PestName == model.PestName))
                {
                    return BadRequest("Pest with such name already exists");
                }

                var newPest = new PestDto
                {
                    PestName = model.PestName,
                    PestDescription = model.PestDescription
                };

                await _context.Pest.AddAsync(newPest);
                await _context.SaveChangesAsync();

                return Ok($"{newPest.PestName} was added successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during adding new pest");
                return StatusCode(500, ex.Message);

            }
        }


        /// <summary>
        /// Add a pest to a plant.
        /// </summary>
        /// <param name="plantId">The ID of the plant to add the pest to.</param>
        /// <param name="pestId">The ID of the pest to add to the plant.</param>
        /// <returns>
        /// If the operation is successful, it will return a message confirming the addition.
        /// If the plant or pest is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpPost("add-to-plant")]
        public async Task<IActionResult> AddPestToPlant(int plantId, int pestId)
        {
            try
            {
                var plant = await _context.Plant.FindAsync(plantId);
                if (plant == null)
                {
                    return NotFound($"Plant with ID {plantId} not found");
                }

                var pest = await _context.Pest.FindAsync(pestId);
                if (pest == null)
                {
                    return NotFound($"Pest with ID {pestId} not found");
                }

                var pestInPlant = new PestInPlantDto
                {
                    PlantId = plantId,
                    PestId = pestId
                };

                _context.Pest_in_Plant.Add(pestInPlant);
                await _context.SaveChangesAsync();

                return Ok($"Pest with ID {pestId} added to plant with ID {plantId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding pest to plant");
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Delete a pest from a plant.
        /// </summary>
        /// <param name="plantId">The ID of the plant to delete the pest from.</param>
        /// <param name="pestId">The ID of the pest to delete from the plant.</param>
        /// <returns>
        /// If the operation is successful, it will return a message confirming the deletion.
        /// If the pest is not associated with the plant, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpDelete("delete-from-plant")]
        public async Task<IActionResult> DeletePestFromPlant(int plantId, int pestId)
        {
            try
            {
                var pestInPlant = await _context.Pest_in_Plant.FirstOrDefaultAsync(pip => pip.PlantId == plantId && pip.PestId == pestId);
                if (pestInPlant == null)
                {
                    return NotFound($"Pest with ID {pestId} is not associated with plant with ID {plantId}");
                }

                _context.Pest_in_Plant.Remove(pestInPlant);
                await _context.SaveChangesAsync();

                return Ok($"Pest with ID {pestId} deleted from plant with ID {plantId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting pest from plant");
                return StatusCode(500, ex.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Fertilizer;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/Fertilizers
    [ApiController]
    [Route("api/[controller]")]

    public class FertilizersController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<FertilizersController> _logger;

        public FertilizersController(GreenGuardDbContext context, ILogger<FertilizersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of all fertilizers.
        /// </summary>
        /// <returns>
        /// If the operation is successful, it will return an ICollection of FertilizerDto.
        /// If there is a bad request, it will return an ErrorDto.
        /// </returns>
        [HttpGet("all-fertilizers")]
        public async Task<IActionResult> GetFertilizers()
        {
            try
            {
                var fertilizers = _context.Fertilizer.Select(data => new FertilizerDto
                {
                    FertilizerId = data.FertilizerId,
                    FertilizerName = data.FertilizerName,
                    FertilizerQuantity = data.FertilizerQuantity
                }).ToList();
                return Ok(fertilizers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all fertilizers loading");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update the quantity of a fertilizer.
        /// </summary>
        /// <param name="id">The ID of the fertilizer to update.</param>
        /// <param name="model">The data to update the fertilizer quantity.</param>
        /// <returns>
        /// If the operation is successful, it will return a message confirming the update.
        /// If there is a bad request, it will return an ErrorDto.
        /// </returns>
        [HttpPut("update-fertilizer-quantity/{id}")]
        public async Task<IActionResult> UpdateFertilizerQuantity(int id, UpdateFertilizerQuantity model)
        {
            try
            {
                var fertilizer = await _context.Fertilizer.FindAsync(id);
                if (fertilizer == null)
                {
                    return NotFound("There is no fertilizer with the provided ID");
                }

                fertilizer.FertilizerQuantity = model.FertilizerQuantity;
                _context.Update(fertilizer);
                await _context.SaveChangesAsync();

                return Ok($"Fertilizer quantity {fertilizer.FertilizerQuantity} updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during updating quantity of fertilizer");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add a new fertilizer.
        /// </summary>
        /// <param name="model">The data to add a new fertilizer.</param>
        /// <returns>
        /// If the operation is successful, it will return a message confirming the addition.
        /// If there is a bad request, it will return an ErrorDto.
        /// </returns>
        [HttpPost("add-fertilizer")]
        public async Task<IActionResult> AddFertilizer(AddFertilizer model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _context.Fertilizer.AnyAsync(data => data.FertilizerName == model.FertilizerName))
                {
                    return BadRequest("Fertilizer with such name already exists");
                }

                var newFertilizer = new FertilizerDto
                {
                    FertilizerName = model.FertilizerName,
                    FertilizerQuantity = model.FertilizerQuantity
                };

                await _context.Fertilizer.AddAsync(newFertilizer);
                await _context.SaveChangesAsync();

                return Ok($"{newFertilizer.FertilizerName} - {newFertilizer.FertilizerQuantity} was added successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during adding new fertilizer");
                return StatusCode(500, ex.Message);

            }
        }

        /// <summary>
        /// Delete a fertilizer.
        /// </summary>
        /// <param name="id">The ID of the fertilizer to delete.</param>
        /// <returns>
        /// If the operation is successful, it will return a message confirming the deletion.
        /// If there is a bad request, it will return an ErrorDto.
        /// </returns>
        [HttpDelete("delete-fertilizer/{id}")]
        public async Task<IActionResult> DeleteFertilizer(int id)
        {
            try
            {
                var fertilizer = await _context.Fertilizer.FindAsync(id);
                if (fertilizer == null)
                {
                    return BadRequest(ModelState);
                }
                _context.Fertilizer.Remove(fertilizer);
                await _context.SaveChangesAsync();

                return Ok($"Fertilizer {fertilizer.FertilizerName} was successfully deleted");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during deleting fertilizer");
                return StatusCode(500, ex.Message);

            }

        }
    }
}

using Microsoft.AspNetCore.Mvc;
using GreenGuard.Models.Fertilizer;
using Microsoft.AspNetCore.Authorization;
using GreenGuard.Helpers;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/Fertilizers
    [ApiController]
    [Route("api/[controller]")]

    public class FertilizersController : ControllerBase
    {
        private readonly FertilizerService _fertilizerService;

        public FertilizersController(FertilizerService fertilizerService)
        {
            _fertilizerService = fertilizerService;
        }

        /// <summary>
        /// Get a list of all fertilizers.
        /// </summary>
        /// <returns>
        /// If the operation is successful, it will return an ICollection of FertilizerDto.
        /// If there is a bad request, it will return an ErrorDto.
        /// </returns>
        [HttpGet("fertilizers")]
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        public async Task<IActionResult> GetFertilizers()
        {
            try
            {
                var fertilizers = await _fertilizerService.GetFertilizers();
                return Ok(fertilizers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get a fertilizer by its ID.
        /// </summary>
        /// <param name="id">The ID of the fertilizer to retrieve.</param>
        /// <returns>
        /// If the fertilizer with the specified ID is found, it will return the FertilizerDto.
        /// If the fertilizer with the specified ID is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFertilizerById(int id)
        {
            try
            {
                var fertilizer = await _fertilizerService.GetFertilizerById(id);
                if (fertilizer == null)
                {
                    return NotFound($"Fertilizer with ID {id} not found");
                }

                return Ok(fertilizer);
            }
            catch (Exception ex)
            {
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
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpPost("add")]
        public async Task<IActionResult> AddFertilizer(AddFertilizer model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _fertilizerService.AddFertilizer(model);
                return Ok("Fertilizer was successfully added");
            }
            catch (Exception ex)
            {
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
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpPut("update-quantity/{id}")]
        public async Task<IActionResult> UpdateFertilizerQuantity(int id, UpdateFertilizerQuantity model)
        {
            try
            {
                await _fertilizerService.UpdateFertilizerQuantity(id, model);
                return Ok($"Fertilizer quantity updated successfully");
            }
            catch (Exception ex)
            {
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
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFertilizer(int id)
        {
            try
            {
                await _fertilizerService.DeleteFertilizer(id);
                return Ok($"Fertilizer with ID {id} was successfully deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
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
        private readonly PestService _pestService;

        public PestsController(PestService pestService)
        {
            _pestService = pestService;
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
                var pests = await _pestService.GetPests();
                return Ok(pests);
            }
            catch (Exception ex)
            {
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
                var pest = await _pestService.GetPestById(pestId);
                if (pest == null)
                {
                    return NotFound($"Pest with ID {pestId} not found");
                }

                return Ok(pest);
            }
            catch (Exception ex)
            {
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

                await _pestService.AddPest(model);
                return Ok("Pest was successfully added");
            }
            catch (Exception ex)
            {
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
        [HttpPost("add/{pestId}/{plantId}")]
        public async Task<IActionResult> AddPestToPlant(int plantId, int pestId)
        {
            try
            {
                await _pestService.AddPestToPlant(plantId, pestId);
                return Ok($"Pest with ID {pestId} added to plant with ID {plantId}");
            }
            catch (Exception ex)
            {
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
        [HttpDelete("delete/{pestId}/{plantId}")]
        public async Task<IActionResult> DeletePestFromPlant(int plantId, int pestId)
        {
            try
            {
                await _pestService.DeletePestFromPlant(plantId, pestId);
                return Ok($"Pest with ID {pestId} deleted from plant with ID {plantId}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

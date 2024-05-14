using Microsoft.AspNetCore.Mvc;
using GreenGuard.Models.PlantType;
using GreenGuard.Helpers;
using Microsoft.AspNetCore.Authorization;
using GreenGuard.Services;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/PlantTypes
    [ApiController]
    [Route("api/[controller]")]
    public class PlantTypesController : ControllerBase
    {
        private readonly PlantTypeService _plantTypeService;

        public PlantTypesController(PlantTypeService plantTypeService)
        {
            _plantTypeService = plantTypeService;
        }

        /// <summary>
        /// Get a list of all plant types.
        /// </summary>
        /// <returns>
        /// If the operation is successful, it will return an ICollection of PlantTypeDto.
        /// If there is a bad request, it will return an ErrorDto.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("plantTypes")]
        public async Task<IActionResult> GetPlantTypes()
        {
            try
            {
                var plantTypes = await _plantTypeService.GetPlantTypes();
                return Ok(plantTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add a new plant type.
        /// </summary>
        /// <param name="model">The data to add a new plant type.</param>
        /// <returns>
        /// If the operation is successful, it will return a message confirming the addition.
        /// If there is a bad request, it will return an ErrorDto.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("add")]
        public async Task<IActionResult> AddPlantType(AddPlantType model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _plantTypeService.AddPlantType(model);
                return Ok("Plant type was successfully added");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

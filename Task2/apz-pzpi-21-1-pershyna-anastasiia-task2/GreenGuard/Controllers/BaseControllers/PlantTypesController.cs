using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.PlantType;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/PlantTypes
    [ApiController]
    [Route("api/[controller]")]
    public class PlantTypesController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<PlantTypesController> _logger;

        public PlantTypesController(GreenGuardDbContext context, ILogger<PlantTypesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of all plant types.
        /// </summary>
        /// <remarks>
        /// If the operation is successful, it will return an ICollection of PlantTypeDto.
        /// If there is a bad request, it will return an ErrorDto.
        /// </remarks>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpGet("all-plantTypes")]
        public async Task<IActionResult> GetPlantTypes()
        {
            try
            {
                var plantTypes = _context.Plant_type.Select(data => new PlantTypeDto
                {
                    PlantTypeId = data.PlantTypeId,
                    PlantTypeName = data.PlantTypeName,
                    PlantTypeDescription = data.PlantTypeDescription,
                    OptHumidity = data.OptHumidity,
                    OptTemp = data.OptTemp,
                    OptLight = data.OptLight,
                    WaterFreq = data.WaterFreq,
                }).ToList();
                return Ok(plantTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all plant types loading");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add a new plant type.
        /// </summary>
        /// <remarks>
        /// If the operation is successful, it will return a message confirming the addition.
        /// If there is a bad request, it will return an ErrorDto.
        /// </remarks>
        /// <param name="model">The data to add a new plant type.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpPost("add-new-plantType")]
        public async Task<IActionResult> AddPlantType(AddPlantType model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _context.Plant_type.AnyAsync(data => data.PlantTypeName == model.PlantTypeName))
                {
                    return BadRequest("Plant type with such name already exists");
                }

                var newPlantType = new PlantTypeDto
                {
                    PlantTypeName = model.PlantTypeName,
                    PlantTypeDescription = model.PlantTypeDescription,
                    OptHumidity = model.OptHumidity,
                    OptTemp = model.OptTemp,
                    OptLight = model.OptLight,
                    WaterFreq = model.WaterFreq,
                };

                await _context.Plant_type.AddAsync(newPlantType);
                await _context.SaveChangesAsync();

                return Ok($"{newPlantType.PlantTypeName} was added successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during adding new plant type");
                return StatusCode(500, ex.Message);

            }
        }
    }
}

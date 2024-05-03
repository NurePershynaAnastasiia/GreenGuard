using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using apz_pzpi_21_1_pershyna_anastasiia_task2.Data;
using apz_pzpi_21_1_pershyna_anastasiia_task2.DTO;
using apz_pzpi_21_1_pershyna_anastasiia_task2.Models;
using apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Controllers
{
    public class PlantsController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<PlantsController> _logger;

        public PlantsController(GreenGuardDbContext context, ILogger<PlantsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Plants/all-plants
        [HttpGet("all-plants")]
        public async Task<IActionResult> GetPlants()
        {
            try
            {
                var plants = _context.Plants.Select(data => new Plant
                {
                    PlantId = data.PlantId,
                    PlantTypeId = data.PlantTypeId,
                    PlantLocation = data.PlantLocation,
                    Humidity = data.Humidity,
                    Temp = data.Temp,
                    Light = data.Light,
                    AdditionalInfo = data.AdditionalInfo,
                    PlantState = data.PlantState
                }).ToList();
                return Ok(plants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all plants loading");
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Plants/add-plant
        [HttpPost("add-plant")]
        public async Task<ActionResult> AddNewPlant(int newPlantTypeId, string newPlantLocation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);

                }

                var newPlant = new Plant
                {
                    PlantTypeId = newPlantTypeId,
                    PlantLocation = newPlantLocation

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

        // Delete: api/Plants/delete-plant/3
        [HttpDelete("delete-plant/{id}")]
        public async Task<IActionResult> DeletePlant(int id)
        {
            try
            {
                var plant = await _context.Plants.FindAsync(id);
                if (plant == null)
                {
                    return BadRequest(ModelState);
                }
                _context.Plants.Remove(plant);
                await _context.SaveChangesAsync();

                return Ok($"Plant with location: {plant.PlantLocation} was successfully deleted");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during deleting plant");
                return StatusCode(500, ex.Message);

            }

        }

        // PUT: api/Plants/update-plant/5
        [HttpPut("update-plant/{id}")]
        public async Task<IActionResult> UpdatePlant(int id, UpdatePlant model)
        {
            try
            {
                var plant = await _context.Plants.FindAsync(id);
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

        // PUT: api/Plants/update-plant-state/3
        [HttpPut("update-plant-state/{id}")]
        public async Task<IActionResult> UpdatePlantState (int id, UpdatePlantState model)
        {
            try
            {
                var plant = await _context.Plants.FindAsync(id);
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
    }
}

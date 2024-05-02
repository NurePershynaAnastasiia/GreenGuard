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

        // GET: api/Fertilizers/all-fertilizers
        [HttpGet("all-fertilizers")]
        public async Task<IActionResult> GetFertilizers()
        {
            try
            {
                var fertilizers = _context.Fertilizers.Select(data => new FertilizerDto
                {
                    FertilizerId = data.FertilizerId,
                    FertilizerName = data.FertilizerName,
                    FertilizerAmount = data.FertilizerQuantity
                }).ToList();
                return Ok(fertilizers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all fertilizers loading");
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Fertilizers/update-fertilizer-quantity/3
        [HttpPut("update-fertilizer-quantity/{id}")]
        public async Task<IActionResult> UpdateFertilizerQuantity(int id, EditFertilizerQuantity model)
        {
            try
            {
                var fertilizer = await _context.Fertilizers.FindAsync(id);
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

        // POST: api/Fertilizers/add-new-fertilizer
        [HttpPost("add-new-fertilizer")]
        public async Task<IActionResult> AddFertilizer(AddFertilizer model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _context.Fertilizers.AnyAsync(data => data.FertilizerName == model.FertilizerName))
                {
                    return BadRequest("Fertilizer with such name already exists");
                }

                var newFertilizer = new Fertilizer
                {
                    FertilizerName = model.FertilizerName,
                    FertilizerQuantity = model.FertilizerQuantity
                };

                await _context.Fertilizers.AddAsync(newFertilizer);
                await _context.SaveChangesAsync();

                return Ok($"{newFertilizer.FertilizerName} - {newFertilizer.FertilizerQuantity} was added successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during adding new fertilizer");
                return StatusCode(500, ex.Message);

            }
        }

        // Delete: api/Fertilizers/delete-fertilizer/3
        [HttpDelete("delete-fertilizer/{id}")]
        public async Task<IActionResult> DeleteFertilizer(int id)
        {
            try
            {
                var fertilizer = await _context.Fertilizers.FindAsync(id);
                if (fertilizer == null)
                {
                    return BadRequest(ModelState);
                }
                _context.Fertilizers.Remove(fertilizer);
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

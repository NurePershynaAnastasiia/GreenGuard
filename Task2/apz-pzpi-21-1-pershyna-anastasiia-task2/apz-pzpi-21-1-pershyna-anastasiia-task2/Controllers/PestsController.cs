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

        // GET: api/Pests/all-pests
        [HttpGet("all-pests")]
        public async Task<IActionResult> GetPests()
        {
            try
            {
                var pests = _context.Pests.Select(data => new PestDto
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

        // POST: api/Pests/add-new-pest
        [HttpPost("add-new-pest")]
        public async Task<IActionResult> AddPest(AddPest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _context.Pests.AnyAsync(data => data.PestName == model.PestName))
                {
                    return BadRequest("Pest with such name already exists");
                }

                var newPest = new Pest
                {
                    PestName = model.PestName,
                    PestDescription = model.PestDescription
                };

                await _context.Pests.AddAsync(newPest);
                await _context.SaveChangesAsync();

                return Ok($"{newPest.PestName} - {newPest.PestDescription} was added successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during adding new pest");
                return StatusCode(500, ex.Message);

            }
        }
    }
}

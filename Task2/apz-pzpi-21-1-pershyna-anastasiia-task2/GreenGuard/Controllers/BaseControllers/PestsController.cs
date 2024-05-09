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
        [HttpGet("pests")]
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        public async Task<IActionResult> GetPests()
        {
            try
            {
                var pests = _context.Pest.Select(data => new PestDto
                {
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

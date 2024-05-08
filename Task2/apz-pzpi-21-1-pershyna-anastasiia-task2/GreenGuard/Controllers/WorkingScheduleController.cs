using GreenGuard.Data;
using GreenGuard.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GreenGuard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkingScheduleController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<WorkingScheduleController> _logger;

        public WorkingScheduleController(GreenGuardDbContext context, ILogger<WorkingScheduleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("worker/{workerId}")]
        public async Task<IActionResult> GetWorkingScheduleByWorkerId(int workerId)
        {
            try
            {
                var workingSchedule = await _context.Working_Schedule.FindAsync(workerId);

                if (workingSchedule == null)
                {
                    return NotFound($"Working schedule not found for worker with ID {workerId}");
                }

                return Ok(workingSchedule);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching working schedule: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("worker/{workerId}")]
        public async Task<IActionResult> UpdateWorkingScheduleByWorkerId(int workerId, WorkingScheduleDto updatedSchedule)
        {
            try
            {
                var existingSchedule = await _context.Working_Schedule.FindAsync(workerId);

                if (existingSchedule == null)
                {
                    return NotFound($"Working schedule not found for worker with ID {workerId}");
                }

                existingSchedule.Monday = updatedSchedule.Monday;
                existingSchedule.Tuesday = updatedSchedule.Tuesday;
                existingSchedule.Wednesday = updatedSchedule.Wednesday;
                existingSchedule.Thursday = updatedSchedule.Thursday;
                existingSchedule.Friday = updatedSchedule.Friday;
                existingSchedule.Saturday = updatedSchedule.Saturday;
                existingSchedule.Sunday = updatedSchedule.Sunday;

                await _context.SaveChangesAsync();

                return Ok("Working schedule updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating working schedule: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

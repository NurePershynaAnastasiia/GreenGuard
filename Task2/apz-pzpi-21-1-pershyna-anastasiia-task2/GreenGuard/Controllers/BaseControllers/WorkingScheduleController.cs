using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.WorkingSchedule;
using Microsoft.AspNetCore.Mvc;

namespace GreenGuard.Controllers.BaseControllers
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

        /// <summary>
        /// Retrieves the working schedule for a specific worker.
        /// </summary>
        /// <param name="workerId">The ID of the worker.</param>
        /// <returns>
        /// If the operation is successful, it will return the working schedule.
        /// If the working schedule is not found, it will return a 404 Not Found response.
        /// If an error occurs, it will return a 500 Internal Server Error response.
        /// </returns>
        [HttpGet("workerSchedule/{workerId}")]
        public async Task<IActionResult> GetWorkingScheduleByWorkerId(int workerId)
        {
            try
            {
                var workingSchedule = _context.Working_Schedule.Select(data => new UpdateWorkingSchedule
                {
                    Monday = data.Monday,
                    Tuesday = data.Tuesday,
                    Wednesday = data.Wednesday,
                    Thursday = data.Thursday,
                    Friday = data.Friday,
                    Saturday = data.Saturday,
                    Sunday = data.Sunday,
                }).ToList();

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

        /// <summary>
        /// Updates the working schedule for a specific worker.
        /// </summary>
        /// <param name="workerId">The ID of the worker.</param>
        /// <param name="updatedSchedule">The updated working schedule.</param>
        /// <returns>
        /// If the operation is successful, it will return a success message.
        /// If the working schedule for the specified worker is not found, it will return a 404 Not Found response.
        /// If an error occurs, it will return a 500 Internal Server Error response.
        /// </returns>
        [HttpPut("update/{workerId}")]
        public async Task<IActionResult> UpdateWorkingScheduleByWorkerId(int workerId, UpdateWorkingSchedule updatedSchedule)
        {
            try
            {
                var existingSchedule = await _context.Working_Schedule.FindAsync(workerId);

                if (existingSchedule == null)
                {
                    // Create a new working schedule if it doesn't exist for the worker
                    existingSchedule = new WorkingScheduleDto
                    {
                        WorkerId = workerId,
                        Monday = updatedSchedule.Monday,
                        Tuesday = updatedSchedule.Tuesday,
                        Wednesday = updatedSchedule.Wednesday,
                        Thursday = updatedSchedule.Thursday,
                        Friday = updatedSchedule.Friday,
                        Saturday = updatedSchedule.Saturday,
                        Sunday = updatedSchedule.Sunday
                    };

                    _context.Working_Schedule.Add(existingSchedule);
                }
                else
                {
                    // Update the existing working schedule
                    existingSchedule.Monday = updatedSchedule.Monday;
                    existingSchedule.Tuesday = updatedSchedule.Tuesday;
                    existingSchedule.Wednesday = updatedSchedule.Wednesday;
                    existingSchedule.Thursday = updatedSchedule.Thursday;
                    existingSchedule.Friday = updatedSchedule.Friday;
                    existingSchedule.Saturday = updatedSchedule.Saturday;
                    existingSchedule.Sunday = updatedSchedule.Sunday;
                }

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

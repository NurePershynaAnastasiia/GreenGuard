using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Helpers;
using GreenGuard.Models.WorkingSchedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenGuard.Controllers.BaseControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkingScheduleController : ControllerBase
    {
        private readonly WorkingScheduleService _workingScheduleService;
        private readonly ILogger<WorkingScheduleController> _logger;

        public WorkingScheduleController(WorkingScheduleService workingScheduleService, ILogger<WorkingScheduleController> logger)
        {
            _workingScheduleService = workingScheduleService;
            _logger = logger;
        }

        /// <summary>
        /// Get the working schedule for a specific worker.
        /// </summary>
        /// <param name="workerId">The ID of the worker.</param>
        /// <returns>
        /// If the operation is successful, it will return the working schedule.
        /// If the working schedule is not found, it will return a 404 Not Found response.
        /// If an error occurs, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("workerSchedule/{workerId}")]
        public async Task<IActionResult> GetWorkingScheduleByWorkerId(int workerId)
        {
            try
            {
                var workingSchedule = await _workingScheduleService.GetWorkingScheduleByWorkerId(workerId);
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
        /// Update the working schedule for a specific worker.
        /// </summary>
        /// <param name="workerId">The ID of the worker.</param>
        /// <param name="updatedSchedule">The updated working schedule.</param>
        /// <returns>
        /// If the operation is successful, it will return a success message.
        /// If the working schedule for the specified worker is not found, it will return a 404 Not Found response.
        /// If an error occurs, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpPut("update/{workerId}")]
        public async Task<IActionResult> UpdateWorkingScheduleByWorkerId(int workerId, UpdateWorkingSchedule updatedSchedule)
        {
            try
            {
                await _workingScheduleService.UpdateWorkingScheduleByWorkerId(workerId, updatedSchedule);
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

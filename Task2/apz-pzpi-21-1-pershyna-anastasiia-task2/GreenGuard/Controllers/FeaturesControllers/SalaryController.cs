using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenGuard.Controllers.FeaturesControllers
{
    // api/Salary
    [ApiController]
    [Route("api/[controller]")]

    public class SalaryController : ControllerBase
    {
        private readonly SalaryService _salaryService;
        private readonly ILogger<SalaryController> _logger;

        public SalaryController(SalaryService salaryService, ILogger<SalaryController> logger)
        {
            _salaryService = salaryService;
            _logger = logger;
        }

        /// <summary>
        /// Calculate the monthly salary for a worker.
        /// </summary>
        /// <param name="workerId">The ID of the worker for whom to calculate the salary.</param>
        /// <returns>
        /// If the calculation is successful, it will return the monthly salary amount.
        /// If there is an error during calculation, it will return an error message.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("{workerId}")]
        public async Task<ActionResult> CalculateSalary(int workerId)
        {
            try
            {
                var weeklySalary = await _salaryService.CalculateMonthlySalary(workerId);
                return Ok(weeklySalary);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during calculating salary");
                return StatusCode(500, ex.Message);
            }
        }
    }
}

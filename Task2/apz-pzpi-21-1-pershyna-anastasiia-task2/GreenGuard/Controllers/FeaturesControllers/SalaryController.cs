using GreenGuard.Data;
using GreenGuard.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GreenGuard.Controllers.FeaturesControllers
{
    // api/PlantTypes
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

        bool CheckNulls(bool? flag)
        {
            return flag == null ? false : (bool)flag;
        }

        /// <summary>
        /// Calculate the weekly salary for a worker.
        /// </summary>
        /// <param name="workerId">The ID of the worker for whom to calculate the salary.</param>
        /// <returns>
        /// If the calculation is successful, it will return the weekly salary amount.
        /// If there is an error during calculation, it will return an error message.
        /// </returns>
        [HttpPost("count-salary")]
        public async Task<ActionResult> CountSalary(int workerId)
        {
            try
            {
                var weeklySalary = await _salaryService.CalculateWeeklySalary(workerId);
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

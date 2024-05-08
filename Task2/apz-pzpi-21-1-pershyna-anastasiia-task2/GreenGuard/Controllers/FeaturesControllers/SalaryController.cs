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

        // GET: api/Salary/count-salary
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

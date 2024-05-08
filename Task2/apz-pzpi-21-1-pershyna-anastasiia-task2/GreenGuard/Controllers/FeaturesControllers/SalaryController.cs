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
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<SalaryController> _logger;

        public SalaryController(GreenGuardDbContext context, ILogger<SalaryController> logger)
        {
            _context = context;
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
                var worker = _context.Worker.FirstOrDefault(w => w.WorkerId == workerId);

                var workingSchedule = _context.Working_Schedule.FirstOrDefault(ws => ws.WorkerId == workerId);

                var tasks = _context.Worker_in_Task.Where(wt => wt.WorkerId == workerId && wt.TaskStatus == "finished").ToList();

                int workingDaysPerWeek = new bool[] { CheckNulls(workingSchedule.Monday), CheckNulls(workingSchedule.Tuesday), CheckNulls(workingSchedule.Wednesday),
                                               CheckNulls(workingSchedule.Thursday), CheckNulls(workingSchedule.Friday), CheckNulls(workingSchedule.Saturday),
                                               CheckNulls(workingSchedule.Sunday) }.Count(d => d == true);

                double hoursPerDay = (worker.EndWorkTime - worker.StartWorkTime).Value.TotalHours;

                double hourlyRate = 150;
                double weeklySalary = workingDaysPerWeek * hoursPerDay * hourlyRate;

                double bonusPerTask = hourlyRate * 0.3;
                double totalBonus = tasks.Count * bonusPerTask;
                weeklySalary += totalBonus;

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

using GreenGuard.Controllers.BaseControllers;
using GreenGuard.Data;

using Microsoft.EntityFrameworkCore;

public class SalaryService
{
    private readonly GreenGuardDbContext _context;

    public SalaryService(GreenGuardDbContext context)
    {
        _context = context;
    }

    public async Task<double> CalculateMonthlySalary(int workerId)
    {
        try
        {
            var worker = await _context.Worker.FirstOrDefaultAsync(w => w.WorkerId == workerId);

            var workingSchedule = await _context.Working_Schedule.FirstOrDefaultAsync(ws => ws.WorkerId == workerId);

            if (worker == null || workingSchedule == null) {
                return 0;
            }

            var tasks = await _context.Worker_in_Task.Where(wt => wt.WorkerId == workerId && wt.TaskStatus == "finished").ToListAsync();

            int workingDaysPerWeek = new bool[] { CheckNulls(workingSchedule.Monday), CheckNulls(workingSchedule.Tuesday), CheckNulls(workingSchedule.Wednesday),
                                           CheckNulls(workingSchedule.Thursday), CheckNulls(workingSchedule.Friday), CheckNulls(workingSchedule.Saturday),
                                           CheckNulls(workingSchedule.Sunday) }.Count(d => d == true);

            double hoursPerDay = (worker.EndWorkTime - worker.StartWorkTime).Value.TotalHours;

            double hourlyRate = 150;
            double workingWeeks = 4;
            double monthlySalary = workingDaysPerWeek * hoursPerDay * hourlyRate * workingWeeks;

            double bonusPerTask = hourlyRate * 0.3;
            double totalBonus = tasks.Count * bonusPerTask;
            monthlySalary += totalBonus;

            return monthlySalary;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred during calculating salary", ex);
        }
    }

    private bool CheckNulls(bool? flag)
    {
        return flag == null ? false : (bool)flag;
    }
}

using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.WorkingSchedule;
using Microsoft.EntityFrameworkCore;

public class WorkingScheduleService
{
    private readonly GreenGuardDbContext _context;

    public WorkingScheduleService(GreenGuardDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateWorkingSchedule> GetWorkingScheduleByWorkerId(int workerId)
    {
        var workingSchedule = await _context.Working_Schedule
            .FirstOrDefaultAsync(ws => ws.WorkerId == workerId);

        if (workingSchedule == null)
        {
            return null;
        }

        return new UpdateWorkingSchedule
        {
            Monday = workingSchedule.Monday,
            Tuesday = workingSchedule.Tuesday,
            Wednesday = workingSchedule.Wednesday,
            Thursday = workingSchedule.Thursday,
            Friday = workingSchedule.Friday,
            Saturday = workingSchedule.Saturday,
            Sunday = workingSchedule.Sunday,
        };
    }

    public async Task UpdateWorkingScheduleByWorkerId(int workerId, UpdateWorkingSchedule updatedSchedule)
    {
        var existingSchedule = await _context.Working_Schedule.FirstOrDefaultAsync(ws => ws.WorkerId == workerId);

        if (existingSchedule == null)
        {
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
            existingSchedule.Monday = updatedSchedule.Monday;
            existingSchedule.Tuesday = updatedSchedule.Tuesday;
            existingSchedule.Wednesday = updatedSchedule.Wednesday;
            existingSchedule.Thursday = updatedSchedule.Thursday;
            existingSchedule.Friday = updatedSchedule.Friday;
            existingSchedule.Saturday = updatedSchedule.Saturday;
            existingSchedule.Sunday = updatedSchedule.Sunday;
        }

        await _context.SaveChangesAsync();
    }
}
using GreenGuard.Controllers.BaseControllers;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Task;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenGuard.Services
{
    public class TaskService
    {
        private readonly GreenGuardDbContext _context;

        public TaskService(GreenGuardDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskFull>> GetTasksWithDetails()
        {
            var tasks = await _context.Task
                .Select(data => new TaskFull
                {
                    TaskId = data.TaskId,
                    TaskDate = data.TaskDate,
                    TaskDetails = data.TaskDetails,
                    TaskType = data.TaskType,
                    TaskState = data.TaskState,
                    FertilizerId = data.FertilizerId
                })
                .ToListAsync();

            foreach (var task in tasks)
            {
                var plantIds = await _context.Plant_in_Task
                    .Where(pit => pit.TaskId == task.TaskId)
                    .Select(pit => pit.PlantId)
                    .ToListAsync();

                var workerIds = await _context.Worker_in_Task
                    .Where(wit => wit.TaskId == task.TaskId)
                    .Select(wit => wit.WorkerId)
                    .ToListAsync();

                var plants = await _context.Plant
                    .Where(plant => plantIds.Contains(plant.PlantId))
                    .Select(plant => plant.PlantLocation)
                    .ToListAsync();

                var workers = await _context.Worker
                    .Where(worker => workerIds.Contains(worker.WorkerId))
                    .Select(worker => worker.WorkerName)
                    .ToListAsync();

                task.Plants = plants;
                task.Workers = workers;
            }

            return tasks;
        }

        public async Task<IActionResult> AddWorkersToTask(int taskId, List<int> workerIds)
        {
            try
            {
                var task = await _context.Task.FindAsync(taskId);
                if (task == null)
                {
                    return new BadRequestObjectResult("Task not found");
                }

                foreach (var workerId in workerIds)
                {
                    var worker = await _context.Worker.FindAsync(workerId);
                    if (worker == null)
                    {
                        return new BadRequestObjectResult($"Worker with id {workerId} not found");
                    }

                    var existingLink = await _context.Worker_in_Task
                        .FirstOrDefaultAsync(wt => wt.TaskId == taskId && wt.WorkerId == workerId);

                    if (existingLink == null)
                    {
                        var workerInTask = new WorkerInTaskDto
                        {
                            TaskId = taskId,
                            WorkerId = workerId
                        };

                        _context.Worker_in_Task.Add(workerInTask);
                    }
                }

                await _context.SaveChangesAsync();

                return new OkObjectResult("Workers successfully added to task");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> AddPlantToTask(int taskId, List<int> plantIds)
        {
            try
            {
                var task = await _context.Task.FindAsync(taskId);
                if (task == null)
                {
                    return new BadRequestObjectResult("Plant not found");
                }

                foreach (var plantId in plantIds)
                {
                    var plant = await _context.Plant.FindAsync(plantId);
                    if (plant == null)
                    {
                        return new BadRequestObjectResult($"Plant with id {plantId} not found");
                    }

                    var existingLink = await _context.Plant_in_Task
                        .FirstOrDefaultAsync(wt => wt.TaskId == taskId && wt.PlantId == plantId);

                    if (existingLink == null)
                    {
                        var plantInTask = new PlantInTaskDto
                        {
                            TaskId = taskId,
                            PlantId = plantId
                        };

                        _context.Plant_in_Task.Add(plantInTask);
                    }
                }

                await _context.SaveChangesAsync();

                return new OkObjectResult("Plants successfully added to task");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

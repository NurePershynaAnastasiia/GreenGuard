using GreenGuard.Controllers.BaseControllers;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Task;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GreenGuard.Services
{
    public class TaskService
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<TaskService> _logger;

        public TaskService(GreenGuardDbContext context, ILogger<TaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<TaskFull>> GetTasksWithDetails()
        {
            var tasks = await _context.Task
                .Select(data => new TaskFull
                {
                    TaskId = data.TaskId,
                    TaskDate = data.TaskDate.ToLocalTime(),
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

        public async Task<string> AddWorkersToTask(int taskId, List<int> workerIds)
        {
            try
            {
                var task = await _context.Task.FindAsync(taskId);
                if (task == null)
                {
                    return "Task not found";
                }

                foreach (var workerId in workerIds)
                {
                    var worker = await _context.Worker.FindAsync(workerId);
                    if (worker == null)
                    {
                        return $"Worker with id {workerId} not found";
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

                return "Workers successfully added to task";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> AddPlantToTask(int taskId, List<int> plantIds)
        {
            try
            {
                var task = await _context.Task.FindAsync(taskId);
                if (task == null)
                {
                    return "Plant not found";
                }

                foreach (var plantId in plantIds)
                {
                    var plant = await _context.Plant.FindAsync(plantId);
                    if (plant == null)
                    {
                        return $"Plant with id {plantId} not found";
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

                return "Plants successfully added to task";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TaskDto>> GetWorkerTasks(int workerId)
        {
            try
            {
                var workerTasks = await _context.Worker_in_Task
                    .Where(wt => wt.WorkerId == workerId)
                    .ToListAsync();

                var taskIds = workerTasks.Select(wt => wt.TaskId).ToList();

                var tasks = await _context.Task
                    .Where(t => taskIds.Contains(t.TaskId))
                    .ToListAsync();

                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching worker tasks");
                throw; 
            }
        }

        public async Task<List<TaskDto>> GetWorkerTasksToday(int workerId)
        {
            try
            {
                DateTime today = DateTime.Today;

                var workerTasks = await _context.Worker_in_Task
                    .Where(wt => wt.WorkerId == workerId)
                    .ToListAsync();

                var taskIds = workerTasks.Select(wt => wt.TaskId).ToList();

                var tasks = await _context.Task
                    .Where(t => taskIds.Contains(t.TaskId) && t.TaskDate.Date == today)
                    .ToListAsync();

                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching worker tasks for today");
                throw; 
            }
        }

        public async Task<List<object>> GetTaskStatuses(int taskId)
        {
            try
            {
                var taskWorkers = await _context.Worker_in_Task
                    .Where(wt => wt.TaskId == taskId)
                    .ToListAsync();

                var workerStatuses = new List<object>();

                foreach (var taskWorker in taskWorkers)
                {
                    var worker = await _context.Worker.FindAsync(taskWorker.WorkerId);
                    if (worker != null)
                    {
                        workerStatuses.Add(new { worker.WorkerName, taskWorker.TaskStatus });
                    }
                }

                return workerStatuses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching task statuses");
                throw; 
            }
        }

        public async Task<Models.Task.TaskStatus> GetTaskWorkerStatus(int taskId, int workerId)
        {
            try
            {
                var taskStatus = await _context.Worker_in_Task
                    .Where(wt => wt.TaskId == taskId && wt.WorkerId == workerId)
                    .Select(wt => wt.TaskStatus) 
                    .FirstOrDefaultAsync();

                Models.Task.TaskStatus result = new Models.Task.TaskStatus();
                result.WorkerTaskStatus = taskStatus;


                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching task status");
                throw;
            }
        }


        public async Task AddTask(AddTask model)
        {
            try
            {
                var newTask = new TaskDto
                {
                    TaskDate = model.TaskDate.ToUniversalTime(),
                    TaskState = model.TaskState,
                    TaskDetails = model.TaskDetails,
                    TaskType = model.TaskType,
                    FertilizerId = (model.FertilizerId == 0) ? null : model.FertilizerId
                };

                _context.Add(newTask);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during adding new task");
                throw; 
            }
        }

        public async Task<bool> UpdateTask(int id, AddTask updatedTask)
        {
            try
            {
                var existingTask = await _context.Task.FindAsync(id);
                if (existingTask == null)
                {
                    return false; 
                }

                existingTask.TaskDate = updatedTask.TaskDate.ToUniversalTime();
                existingTask.TaskDetails = updatedTask.TaskDetails;
                existingTask.TaskType = updatedTask.TaskType;
                existingTask.TaskState = updatedTask.TaskState;
                existingTask.FertilizerId = updatedTask.FertilizerId;

                _context.Entry(existingTask).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating task");
                throw; 
            }
        }

        public async Task<bool> UpdateTaskStatus(int taskId, int workerId, string taskStatus)
        {
            try
            {
                var workerInTask = await _context.Worker_in_Task
                    .FirstOrDefaultAsync(wt => wt.TaskId == taskId && wt.WorkerId == workerId);

                if (workerInTask == null)
                {
                    return false; 
                }

                workerInTask.TaskStatus = taskStatus;
                await _context.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating task status");
                throw;
            }
        }

        public async Task<bool> DeleteTask(int taskId)
        {
            try
            {
                var task = await _context.Task.FindAsync(taskId);
                if (task == null)
                {
                    return false; 
                }
                _context.Task.Remove(task);
                await _context.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during deleting task");
                throw; 
            }
        }

        public async Task<bool> DeleteWorkerFromTask(int taskId, int workerId)
        {
            try
            {
                var existingLink = await _context.Worker_in_Task
                    .FirstOrDefaultAsync(wt => wt.TaskId == taskId && wt.WorkerId == workerId);

                if (existingLink == null)
                {
                    return false; 
                }

                _context.Worker_in_Task.Remove(existingLink);
                await _context.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting worker from task");
                throw; 
            }
        }

        public async Task<bool> DeletePlantFromTask(int taskId, int plantId)
        {
            try
            {
                var existingLink = await _context.Plant_in_Task
                    .FirstOrDefaultAsync(wt => wt.TaskId == taskId && wt.PlantId == plantId);

                if (existingLink == null)
                {
                    return false; 
                }

                _context.Plant_in_Task.Remove(existingLink);
                await _context.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting plant from task");
                throw; 
            }
        }
    }
}

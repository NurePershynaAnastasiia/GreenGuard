using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Task;
using TaskDto = GreenGuard.Dto.TaskDto;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/Tasks
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<TasksController> _logger;

        public TasksController(GreenGuardDbContext context, ILogger<TasksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Tasks/all-task
        [HttpGet("all-task")]
        public async Task<IActionResult> GetTasks()
        {
            try
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

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all tasks loading");
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Tasks/add-task
        [HttpPost("add-task")]
        public async Task<ActionResult> AddTask(DateTime taskDate, string taskDetails, string taskType, string taskState, int? fertilizerId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);

                }
                var newTask = new TaskDto
                {
                    TaskDate = taskDate,
                    TaskState = taskState,
                    TaskDetails = taskDetails,
                    TaskType = taskType,
                    FertilizerId = fertilizerId
                };

                _context.Add(newTask);
                await _context.SaveChangesAsync();
                return Ok("Task was succesfully created");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during adding new task");
                return StatusCode(500, ex.Message);
            }
        }

        // Delete: api/Tasks/delete-task/3
        [HttpDelete("delete-task/{id}")]
        public async Task<IActionResult> Deletetask(int id)
        {
            try
            {
                var task = await _context.Task.FindAsync(id);
                if (task == null)
                {
                    return BadRequest(ModelState);
                }
                _context.Task.Remove(task);
                await _context.SaveChangesAsync();

                return Ok($"Task with description: {task.TaskDetails} was successfully deleted");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during deleting task");
                return StatusCode(500, ex.Message);

            }

        }

        // POST: api/Tasks/add-worker-to-task/3
        [HttpPost("add-worker-to-task/{taskId}")]
        public async Task<IActionResult> AddWorkerToTask(int taskId, List<int> workerIds)
        {
            try
            {
                var task = await _context.Task.FindAsync(taskId);
                if (task == null)
                {
                    return BadRequest("Task not found");
                }

                foreach (var workerId in workerIds)
                {
                    var worker = await _context.Worker.FindAsync(workerId);
                    if (worker == null)
                    {
                        return BadRequest($"Worker with id {workerId} not found");
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

                return Ok("Workers successfully added to task");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding workers to task");
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Tasks/add-plant-to-task/3
        [HttpPost("add-plant-to-task/{taskId}")]
        public async Task<IActionResult> AddPlantToTask(int taskId, List<int> plantIds)
        {
            try
            {
                var task = await _context.Task.FindAsync(taskId);
                if (task == null)
                {
                    return BadRequest("Task not found");
                }

                foreach (var plantId in plantIds)
                {
                    var plant = await _context.Plant.FindAsync(plantId);
                    if (plant == null)
                    {
                        return BadRequest($"Plant with id {plantId} not found");
                    }

                    var existingLink = await _context.Worker_in_Task
                        .FirstOrDefaultAsync(wt => wt.TaskId == taskId && wt.WorkerId == plantId);

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

                return Ok("Plants successfully added to task");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding workers to task");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Tasks/worker-tasks/3
        [HttpGet("worker-tasks/{workerId}")]
        public async Task<IActionResult> GetWorkerTasks(int workerId)
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

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching worker tasks");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Tasks/worker-tasks-today/3
        [HttpGet("worker-tasks-today/{workerId}")]
        public async Task<IActionResult> GetWorkerTasksToday(int workerId)
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

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching worker tasks for today");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Tasks/task-status/{taskId}
        [HttpGet("task-status/{taskId}")]
        public async Task<IActionResult> GetTaskStatus(int taskId)
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

                return Ok(workerStatuses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching task statuses");
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Tasks/update-task/3
        [HttpPut("update-task/{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskDto updatedTask)
        {
            try
            {
                if (id != updatedTask.TaskId)
                {
                    return BadRequest("Task ID mismatch");
                }

                var existingTask = await _context.Task.FindAsync(id);
                if (existingTask == null)
                {
                    return NotFound("Task not found");
                }

                existingTask.TaskDate = updatedTask.TaskDate;
                existingTask.TaskDetails = updatedTask.TaskDetails;
                existingTask.TaskType = updatedTask.TaskType;
                existingTask.TaskState = updatedTask.TaskState;
                existingTask.FertilizerId = updatedTask.FertilizerId;

                _context.Entry(existingTask).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Task updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating task");
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Tasks/delete-worker-from-task/3
        [HttpDelete("delete-worker-from-task/{taskId}/{workerId}")]
        public async Task<IActionResult> DeleteWorkerFromTask(int taskId, int workerId)
        {
            try
            {
                var existingLink = await _context.Worker_in_Task
                    .FirstOrDefaultAsync(wt => wt.TaskId == taskId && wt.WorkerId == workerId);

                if (existingLink == null)
                {
                    return NotFound("Worker-task link not found");
                }

                _context.Worker_in_Task.Remove(existingLink);
                await _context.SaveChangesAsync();

                return Ok("Worker successfully removed from task");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting worker from task");
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Tasks/delete-plant-from-task/3
        [HttpDelete("delete-plant-from-task/{taskId}/{plantId}")]
        public async Task<IActionResult> DeletePlantFromTask(int taskId, int plantId)
        {
            try
            {
                var existingLink = await _context.Plant_in_Task
                    .FirstOrDefaultAsync(wt => wt.TaskId == taskId && wt.PlantId == plantId);

                if (existingLink == null)
                {
                    return NotFound("Plant-task link not found");
                }

                _context.Plant_in_Task.Remove(existingLink);
                await _context.SaveChangesAsync();

                return Ok("Plant successfully removed from task");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting plant from task");
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Tasks/update-task-status/3/4
        [HttpPut("update-task-status/{taskId}/{workerId}")]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, int workerId, string taskStatus)
        {
            try
            {
                var workerInTask = await _context.Worker_in_Task
                    .FirstOrDefaultAsync(wt => wt.TaskId == taskId && wt.WorkerId == workerId);

                if (workerInTask == null)
                {
                    return NotFound("Worker-task relationship not found");
                }

                workerInTask.TaskStatus = taskStatus;
                await _context.SaveChangesAsync();

                return Ok("Task status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating task status");
                return StatusCode(500, ex.Message);
            }
        }

    }
}

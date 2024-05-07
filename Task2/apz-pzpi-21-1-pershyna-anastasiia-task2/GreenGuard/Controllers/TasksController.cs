using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GreenGuard.Data;
using GreenGuard.DTO;
using GreenGuard.Models;
using GreenGuard.Models.Database;
using Task = GreenGuard.Models.Database.Task;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GreenGuard.Controllers
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

        // GET: api/Tasks/all-tasks
        [HttpGet("all-task")]
        public async Task<IActionResult> GetTasks()
        {
            try
            {
                var tasks = _context.Task.Select(data => new Task
                {
                    TaskId = data.TaskId,
                    TaskDate = data.TaskDate,
                    TaskDetails = data.TaskDetails,
                    TaskType = data.TaskType,
                    TaskState = data.TaskState,
                    FertilizerId = data.FertilizerId
                }).ToList();
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

                var newTask = new Task
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
                        var workerInTask = new WorkerInTask
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
                        var plantInTask = new PlantInTask
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
    }
}

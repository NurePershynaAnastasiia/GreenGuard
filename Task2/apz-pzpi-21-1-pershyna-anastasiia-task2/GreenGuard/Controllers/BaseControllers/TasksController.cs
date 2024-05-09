using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Task;
using TaskDto = GreenGuard.Dto.TaskDto;
using GreenGuard.Services;
using GreenGuard.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/Tasks
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<TasksController> _logger;
        private readonly TaskService _taskService;

        public TasksController(GreenGuardDbContext context, ILogger<TasksController> logger, TaskService taskService)
        {
            _context = context;
            _logger = logger;
            _taskService = taskService;
        }

        /// <summary>
        /// Retrieves a list of all tasks along with associated plants and workers.
        /// </summary>
        /// <returns>
        /// If retrieval is successful, it returns a list of TaskFull objects containing task details, associated plants, and workers.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("tasks")]
        public async Task<IActionResult> GetTasks()
        {
            try
            {
                var tasks = await _taskService.GetTasksWithDetails();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all tasks loading");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves tasks assigned to a specific worker.
        /// </summary>
        /// <param name="workerId">The ID of the worker.</param>
        /// <returns>
        /// If retrieval is successful, it returns a list of tasks assigned to the worker.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
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

        /// <summary>
        /// Retrieves tasks assigned to a specific worker for today's date.
        /// </summary>
        /// <param name="workerId">The ID of the worker.</param>
        /// <returns>
        /// If retrieval is successful, it returns a list of tasks assigned to the worker for today.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
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

        /// <summary>
        /// Retrieves the statuses of workers associated with a specific task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>
        /// If retrieval is successful, it returns a list of objects containing worker names and their task statuses.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("statuses/{taskId}")]
        public async Task<IActionResult> GetTaskStatuses(int taskId)
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

        /// <summary>
        /// Adds a new task.
        /// </summary>
        /// <param name="model">The AddTask model containing task details.</param>
        /// <returns>
        /// If addition is successful, it returns a success message.
        /// If the model is invalid, it returns a 400 Bad Request response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("add")]
        public async Task<ActionResult> AddTask(AddTask model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);

                }
                var newTask = new TaskDto
                {
                    TaskDate = model.TaskDate,
                    TaskState = model.TaskState,
                    TaskDetails = model.TaskDetails,
                    TaskType = model.TaskType,
                    FertilizerId = model.FertilizerId
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

        /// <summary>
        /// Adds workers to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="workerIds">The IDs of the workers to add.</param>
        /// <returns>
        /// If addition is successful, it returns a success message.
        /// If the task with the provided ID does not exist, it returns a 400 Bad Request response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("add-worker/{taskId}")]
        public async Task<IActionResult> AddWorkerToTask(int taskId, List<int> workerIds)
        {
            try
            {
                var result = await _taskService.AddWorkersToTask(taskId, workerIds);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding workers to task");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Adds plants to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="plantIds">The IDs of the plants to add.</param>
        /// <returns>
        /// If addition is successful, it returns a success message.
        /// If the task with the provided ID does not exist, it returns a 400 Bad Request response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("add-plant/{taskId}")]
        public async Task<IActionResult> AddPlantToTask(int taskId, List<int> plantIds)
        {
            try
            {
                var result = await _taskService.AddPlantToTask(taskId, plantIds);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding workers to task");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates a task by ID.
        /// </summary>
        /// <param name="id">The ID of the task to update.</param>
        /// <param name="updatedTask">The updated TaskDto object.</param>
        /// <returns>
        /// If update is successful, it returns a success message.
        /// If the task with the provided ID does not exist, it returns a 404 Not Found response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTask(int id, AddTask updatedTask)
        {
            try
            {
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

        /// <summary>
        /// Updates the status of a worker associated with a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="workerId">The ID of the worker.</param>
        /// <param name="taskStatus">The new task status for the worker.</param>
        /// <returns>
        /// If update is successful, it returns a success message.
        /// If the worker-task relationship does not exist, it returns a 404 Not Found response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpPut("update-status/{taskId}/{workerId}")]
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

        /// <summary>
        /// Deletes a task by ID.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <returns>
        /// If deletion is successful, it returns a success message.
        /// If the task with the provided ID does not exist, it returns a 400 Bad Request response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
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

        /// <summary>
        /// Deletes a worker from a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="workerId">The ID of the worker to delete from the task.</param>
        /// <returns>
        /// If deletion is successful, it returns a success message.
        /// If the worker-task link does not exist, it returns a 404 Not Found response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
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

        /// <summary>
        /// Deletes a plant from a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="plantId">The ID of the plant to delete from the task.</param>
        /// <returns>
        /// If deletion is successful, it returns a success message.
        /// If the plant-task link does not exist, it returns a 404 Not Found response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
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
    }
}

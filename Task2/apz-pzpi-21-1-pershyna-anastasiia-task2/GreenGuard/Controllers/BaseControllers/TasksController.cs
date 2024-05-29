using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Task;
using TaskDto = GreenGuard.Dto.TaskDto;
using GreenGuard.Services;
using GreenGuard.Helpers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/Tasks
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ILogger<TasksController> _logger;
        private readonly TaskService _taskService;

        public TasksController(ILogger<TasksController> logger, TaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        /// <summary>
        /// Get a list of all tasks along with associated plants and workers.
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
        /// Get tasks assigned to a specific worker.
        /// </summary>
        /// <param name="workerId">The ID of the worker.</param>
        /// <returns>
        /// If retrieval is successful, it returns a list of tasks assigned to the worker.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("tasks/{workerId}")]
        public async Task<IActionResult> GetWorkerTasks(int workerId)
        {
            try
            {
                var tasks = await _taskService.GetWorkerTasks(workerId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get tasks assigned to a specific worker for today's date.
        /// </summary>
        /// <param name="workerId">The ID of the worker.</param>
        /// <returns>
        /// If retrieval is successful, it returns a list of tasks assigned to the worker for today.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("tasks-today/{workerId}")]
        public async Task<IActionResult> GetWorkerTasksToday(int workerId)
        {
            try
            {
                var tasks = await _taskService.GetWorkerTasksToday(workerId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get the statuses of workers associated with a specific task.
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
                var workerStatuses = await _taskService.GetTaskStatuses(taskId);
                return Ok(workerStatuses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get the status of a worker associated with a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="workerId">The ID of the worker.</param>
        /// <returns>
        /// If update is successful, it returns a success message.
        /// If the worker-task relationship does not exist, it returns a 404 Not Found response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("status/{taskId}/{workerId}")]
        public async Task<IActionResult> GetTaskStatus(int taskId, int workerId)
        {
            try
            {
                var result = await _taskService.GetTaskWorkerStatus(taskId, workerId);
                _logger.LogInformation(result.WorkerTaskStatus);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add a new task.
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
                    return BadRequest(ModelState);

                await _taskService.AddTask(model);
                return Ok("Task was succesfully created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add workers to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="workerIds">The IDs of the workers to add.</param>
        /// <returns>
        /// If addition is successful, it returns a success message.
        /// If the task with the provided ID does not exist, it returns a 400 Bad Request response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("add-workers/{taskId}")]
        public async Task<IActionResult> AddWorkersToTask(int taskId, List<int> workerIds)
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
        /// Add plants to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="plantIds">The IDs of the plants to add.</param>
        /// <returns>
        /// If addition is successful, it returns a success message.
        /// If the task with the provided ID does not exist, it returns a 400 Bad Request response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("add-plants/{taskId}")]
        public async Task<IActionResult> AddPlantsToTask(int taskId, List<int> plantIds)
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
        /// Update a task by ID.
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
                var result = await _taskService.UpdateTask(id, updatedTask);
                if (result)
                    return Ok("Task updated successfully");
                else
                    return NotFound("Task not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update the status of a worker associated with a task.
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
                var result = await _taskService.UpdateTaskStatus(taskId, workerId, taskStatus);
                if (result)
                    return Ok("Task status updated successfully");
                else
                    return NotFound("Worker-task relationship not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete a task by ID.
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
                var result = await _taskService.DeleteTask(id);
                if (result)
                    return Ok($"Task with ID {id} was successfully deleted");
                else
                    return NotFound($"Task with ID {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete a worker from a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="workerId">The ID of the worker to delete from the task.</param>
        /// <returns>
        /// If deletion is successful, it returns a success message.
        /// If the worker-task link does not exist, it returns a 404 Not Found response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("delete-worker/{taskId}/{workerId}")]
        public async Task<IActionResult> e(int taskId, int workerId)
        {
            try
            {
                var result = await _taskService.DeleteWorkerFromTask(taskId, workerId);
                if (result)
                    return Ok("Worker successfully removed from task");
                else
                    return NotFound("Worker-task link not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete a plant from a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="plantId">The ID of the plant to delete from the task.</param>
        /// <returns>
        /// If deletion is successful, it returns a success message.
        /// If the plant-task link does not exist, it returns a 404 Not Found response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("delete-plant/{taskId}/{plantId}")]
        public async Task<IActionResult> DeletePlantFromTask(int taskId, int plantId)
        {
            try
            {
                var result = await _taskService.DeletePlantFromTask(taskId, plantId);
                if (result)
                {
                    return Ok("Plant successfully removed from task");
                }
                else
                {
                    return NotFound("Plant-task link not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}

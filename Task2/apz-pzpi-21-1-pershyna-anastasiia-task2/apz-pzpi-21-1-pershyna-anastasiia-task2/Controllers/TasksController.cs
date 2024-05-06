using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using apz_pzpi_21_1_pershyna_anastasiia_task2.Data;
using apz_pzpi_21_1_pershyna_anastasiia_task2.DTO;
using apz_pzpi_21_1_pershyna_anastasiia_task2.Models;
using apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database;
using Task = apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database.Task;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Controllers
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
        public async Task<ActionResult> AddTask(DateTime taskDate, string taskDetails, string taskType, string taskState, int fertilizerId)
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
    }
}

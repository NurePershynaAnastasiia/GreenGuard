using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenGuard.Data;
using GreenGuard.Dto;
using Microsoft.AspNetCore.Identity;
using GreenGuard.Services;
using GreenGuard.Models.Worker;
using Microsoft.AspNetCore.Authorization;
using GreenGuard.Helpers;
using Microsoft.SqlServer.Management.Smo.Broker;
using static GreenGuard.Helpers.DateValidation;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/Workers
    [ApiController]
    [Route("api/[controller]")]
    public class WorkersController : ControllerBase
    {
        private readonly WorkerService _workerService;

        public WorkersController(WorkerService workerService)
        {
            _workerService = workerService;
        }

        /// <summary>
        /// Get a list of all workers.
        /// </summary>
        /// <returns>
        /// If the operation is successful, it will return an ICollection of WorkerDto.
        /// If there is an error during retrieval, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("workers")]
        public async Task<IActionResult> GetWorkers()
        {
            try
            {
                var workers = await _workerService.GetWorkers();
                return Ok(workers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get a worker by id
        /// </summary>
        /// <returns>
        /// <param name="id">The ID of the worker</param>
        /// If the operation is successful, it will return a GetWorker object
        /// If there is an error during retrieval, it will return a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpGet("workers/{id}")]
        public async Task<IActionResult> GetWorker(int id)
        {
            try
            {
                var worker = await _workerService.GetWorker(id);
                return Ok(worker);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get workers who are scheduled to work on a specific date.
        /// </summary>
        /// <param name="date">The date to retrieve workers for.</param>
        /// <returns>
        /// If retrieval is successful, it returns a list of workers scheduled to work on the specified date.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("working-date")]
        public async Task<IActionResult> GetWorkersWorkingOnDate(string dateString)
        {
            if (TryParseDate(dateString, out DateTime date))
            {
                try
                {
                    var workers = await _workerService.GetWorkersWorkingOnDate(date);
                    return Ok(workers);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            else
            {
                return BadRequest("Невірний формат дати");
            }
        }

        /// <summary>
        /// Authenticate a worker using email and password.
        /// </summary>
        /// <param name="model">The WorkerLogin model containing email and password.</param>
        /// <returns>
        /// If authentication is successful, it returns a JWT token.
        /// If the email or password is invalid, it returns a 400 Bad Request response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> WorkerLogin(WorkerLogin model)
        {
            try
            {
                var token = await _workerService.AuthenticateWorker(model);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Register a new worker.
        /// </summary>
        /// <param name="model">The WorkerRegister model containing worker information.</param>
        /// <returns>
        /// If registration is successful, it returns a JWT token.
        /// If the email is already in use, it returns a 400 Bad Request response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("register")]
        public async Task<IActionResult> WorkerRegister(WorkerRegister model)
        {
            try
            {
                await _workerService.RegisterWorker(model);
                return Ok("New worker was successfully registered");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update worker information by ID.
        /// </summary>
        /// <param name="id">The ID of the worker to update.</param>
        /// <param name="updatedWorker">The updated worker information.</param>
        /// <returns>
        /// If update is successful, it returns a success message.
        /// If the worker with the provided ID does not exist, it returns a 404 Not Found response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator + "," + Roles.User)]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateWorker(int id, UpdateWorker updatedWorker)
        {
            try
            {
                await _workerService.UpdateWorker(id, updatedWorker);
                return Ok("Worker information updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update the role of a worker by ID.
        /// </summary>
        /// <param name="id">The ID of the worker to update.</param>
        /// <param name="isAdmin">Boolean value indicating whether the worker is an administrator.</param>
        /// <returns>
        /// If update is successful, it returns a success message.
        /// If the worker with the provided ID does not exist, it returns a 404 Not Found response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPut("update-role/{id}")]
        public async Task<IActionResult> UpdateWorkerRole(int id, bool isAdmin)
        {
            try
            {
                await _workerService.UpdateWorkerRole(id, isAdmin);
                return Ok("Worker role updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete a worker by ID.
        /// </summary>
        /// <param name="id">The ID of the worker to delete.</param>
        /// <returns>
        /// If deletion is successful, it returns a success message.
        /// If the worker with the provided ID does not exist, it returns a 404 Not Found response.
        /// If an error occurs, it returns a 500 Internal Server Error response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            try
            {
                await _workerService.DeleteWorker(id);
                return Ok($"Worker with ID {id} successfully deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

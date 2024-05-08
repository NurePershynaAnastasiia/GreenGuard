using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenGuard.Data;
using GreenGuard.Dto;
using Microsoft.AspNetCore.Identity;
using GreenGuard.Services;
using GreenGuard.Models.Worker;
using Microsoft.AspNetCore.Authorization;
using GreenGuard.Helpers;

namespace GreenGuard.Controllers.BaseControllers
{
    // api/Workers
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkersController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<WorkersController> _logger;
        private readonly IPasswordHasher<WorkerDto> _passwordHasher;
        private readonly JwtTokenGenerator _jwtService;

        public WorkersController(GreenGuardDbContext context, ILogger<WorkersController> logger, IConfiguration config, IPasswordHasher<WorkerDto> passwordHasher)
        {
            _jwtService = new JwtTokenGenerator(config);
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Workers/all-workers
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("all-workers")]
        public async Task<IActionResult> GetWorkers()
        {
            try
            {
                var workers = _context.Worker.Select(data => new WorkerDto
                {
                    WorkerId = data.WorkerId,
                    WorkerName = data.WorkerName,
                    PhoneNumber = data.PhoneNumber,
                    StartWorkTime = data.StartWorkTime,
                    EndWorkTime = data.EndWorkTime,
                    Email = data.Email,
                    PasswordHash = data.PasswordHash,
                }).ToList();
                return Ok(workers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all workers loading");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> WorkerLogin(WorkerLogin model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var worker = await _context.Worker.FirstOrDefaultAsync(w => w.Email == model.Email);

                if (worker == null)
                {
                    return BadRequest("Invalid email or password");
                }

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(worker, worker.PasswordHash, model.Password);

                if (passwordVerificationResult != PasswordVerificationResult.Success)
                {
                    return BadRequest("Invalid email or password");
                }

                var token = _jwtService.GenerateToken(worker);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> WorkerRegister(WorkerRegister model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _context.Worker.AnyAsync(w => w.Email == model.Email))
                {
                    return BadRequest("Worker with such email already exists");
                }

                var newWorker = new WorkerDto
                {
                    WorkerName = model.WorkerName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    IsAdmin = model.IsAdmin,
                };

                newWorker.PasswordHash = _passwordHasher.HashPassword(newWorker, model.Password);

                await _context.Worker.AddAsync(newWorker);
                await _context.SaveChangesAsync();

                var token = _jwtService.GenerateToken(newWorker);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during worker registration");
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Workers/delete-worker/3
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("delete-worker/{id}")]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            try
            {
                var worker = await _context.Worker.FindAsync(id);
                if (worker == null)
                {
                    return NotFound("Worker not found");
                }

                _context.Worker.Remove(worker);
                await _context.SaveChangesAsync();

                return Ok($"Worker with ID {id} successfully deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting worker");
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Workers/update-worker/3
        [HttpPut("update-worker/{id}")]
        public async Task<IActionResult> UpdateWorker(int id, WorkerDto updatedWorker)
        {
            try
            {
                var existingWorker = await _context.Worker.FindAsync(id);
                if (existingWorker == null)
                {
                    return NotFound("Worker not found");
                }

                existingWorker.WorkerName = updatedWorker.WorkerName;
                existingWorker.Email = updatedWorker.Email;
                existingWorker.PhoneNumber = updatedWorker.PhoneNumber;
                existingWorker.StartWorkTime = updatedWorker.StartWorkTime;
                existingWorker.EndWorkTime = updatedWorker.EndWorkTime;

                _context.Entry(existingWorker).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Worker information updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating worker information");
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Workers/update-worker-role/3
        [Authorize(Roles = Roles.Administrator)]
        [HttpPut("update-worker-role/{id}")]
        public async Task<IActionResult> UpdateWorkerRole(int id, bool isAdmin)
        {
            try
            {

                var existingWorker = await _context.Worker.FindAsync(id);
                if (existingWorker == null)
                {
                    return NotFound("Worker not found");
                }

                existingWorker.IsAdmin = isAdmin;

                _context.Entry(existingWorker).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Worker role updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating worker role");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("working-at/{date}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> GetWorkersWorkingAtDate(DateTime date)
        {
            try
            {
                var workers = await _context.Working_Schedule
                    .Where(ws =>
                        ws.Monday == true && date.DayOfWeek == DayOfWeek.Monday ||
                        ws.Tuesday == true && date.DayOfWeek == DayOfWeek.Tuesday ||
                        ws.Wednesday == true && date.DayOfWeek == DayOfWeek.Wednesday ||
                        ws.Thursday == true && date.DayOfWeek == DayOfWeek.Thursday ||
                        ws.Friday == true && date.DayOfWeek == DayOfWeek.Friday ||
                        ws.Saturday == true && date.DayOfWeek == DayOfWeek.Saturday ||
                        ws.Sunday == true && date.DayOfWeek == DayOfWeek.Sunday
                    )
                    .Select(ws => ws.WorkerId)
                    .ToListAsync();

                var workersInfo = await _context.Worker
                    .Where(w => workers.Contains(w.WorkerId))
                    .ToListAsync();

                return Ok(workersInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching workers working at the specified date");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

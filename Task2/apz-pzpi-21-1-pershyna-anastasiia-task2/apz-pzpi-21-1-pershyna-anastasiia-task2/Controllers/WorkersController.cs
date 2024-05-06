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
using Microsoft.AspNetCore.Identity;
using apz_pzpi_21_1_pershyna_anastasiia_task2.Services;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Controllers
{
    // api/Workers
    [ApiController]
    [Route("api/[controller]")]
    public class WorkersController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<WorkersController> _logger;
        private readonly IPasswordHasher<Worker> _passwordHasher;
        private readonly JwtService _jwtService;

        public WorkersController(GreenGuardDbContext context, ILogger<WorkersController> logger, IConfiguration config, IPasswordHasher<Worker> passwordHasher)
        {
            _jwtService = new JwtService(config);
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Workers/all-workers
        [HttpGet("all-workers")]
        public async Task<IActionResult> GetWorkers()
        {
            try
            {
                var workers = _context.Worker.Select(data => new Worker
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

                var token = _jwtService.GenerateToken(worker.WorkerId);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> AddWorker(WorkerRegister model)
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

                var newWorker = new Worker
                {
                    WorkerName = model.WorkerName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    IsAdmin = model.IsAdmin, 
                };

                newWorker.PasswordHash = _passwordHasher.HashPassword(null, model.Password);

                await _context.Worker.AddAsync(newWorker);
                await _context.SaveChangesAsync();

                var token = _jwtService.GenerateToken(newWorker.WorkerId);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during worker registration");
                return StatusCode(500, ex.Message);
            }
        }
        
    }
}

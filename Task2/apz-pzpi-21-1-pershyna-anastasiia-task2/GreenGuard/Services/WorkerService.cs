using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Fertilizer;
using GreenGuard.Models.Worker;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GreenGuard.Services
{
    public class WorkerService
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<WorkerService> _logger;
        private readonly IPasswordHasher<WorkerDto> _passwordHasher;
        private readonly JwtTokenService _jwtService;

        public WorkerService(GreenGuardDbContext context, ILogger<WorkerService> logger, IConfiguration config, IPasswordHasher<WorkerDto> passwordHasher)
        {
            _jwtService = new JwtTokenService(config);
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<GetWorker>> GetWorkers()
        {
            try
            {
                var workers = await _context.Worker.Select(data => new GetWorker
                {
                    WorkerId = data.WorkerId,
                    WorkerName = data.WorkerName,
                    PhoneNumber = data.PhoneNumber,
                    StartWorkTime = data.StartWorkTime,
                    EndWorkTime = data.EndWorkTime,
                    Email = data.Email,
                    IsAdmin = data.IsAdmin,
                }).ToListAsync();
                return workers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all workers loading");
                throw;
            }
        }

        public async Task<GetWorker> GetWorker(int id)
        {
            try
            {
                var worker = await _context.Worker.FindAsync(id);
                if (worker == null)
                {
                    return null;
                }

                return new GetWorker
                {
                    WorkerId = worker.WorkerId,
                    WorkerName = worker.WorkerName,
                    PhoneNumber = worker.PhoneNumber,
                    StartWorkTime = worker.StartWorkTime,
                    EndWorkTime = worker.EndWorkTime,
                    Email = worker.Email,
                    IsAdmin = worker.IsAdmin
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during all workers loading");
                throw;
            }
        }

        public async Task<IEnumerable<WorkerDto>> GetWorkersWorkingOnDate(DateTime date)
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

                return workersInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching workers working at the specified date");
                throw;
            }
        }

        public async Task<string> AuthenticateWorker(WorkerLogin model)
        {
            try
            {
                var worker = await _context.Worker.FirstOrDefaultAsync(w => w.Email == model.Email);

                if (worker == null || _passwordHasher.VerifyHashedPassword(worker, worker.PasswordHash, model.Password) != PasswordVerificationResult.Success)
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }

                return _jwtService.GenerateToken(worker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during worker authentication");
                throw;
            }
        }

        public async Task RegisterWorker(WorkerRegister model)
        {
            try
            {
                if (await _context.Worker.AnyAsync(w => w.Email == model.Email))
                {
                    throw new InvalidOperationException("Worker with such email already exists");
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during worker registration");
                throw;
            }
        }

        public async Task UpdateWorker(int id, UpdateWorker updatedWorker)
        {
            try
            {
                var existingWorker = await _context.Worker.FindAsync(id);
                if (existingWorker == null)
                {
                    throw new Exception("Worker not found");
                }

                existingWorker.WorkerName = updatedWorker.WorkerName;
                existingWorker.Email = updatedWorker.Email;
                existingWorker.PhoneNumber = updatedWorker.PhoneNumber;
                existingWorker.StartWorkTime = updatedWorker.StartWorkTime;
                existingWorker.EndWorkTime = updatedWorker.EndWorkTime;
                existingWorker.IsAdmin = updatedWorker.isAdmin;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating worker information");
                throw;
            }
        }

        public async Task UpdateWorkerRole(int id, bool isAdmin)
        {
            try
            {
                var existingWorker = await _context.Worker.FindAsync(id);
                if (existingWorker == null)
                {
                    throw new Exception("Worker not found");
                }

                existingWorker.IsAdmin = isAdmin;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating worker role");
                throw;
            }
        }

        public async Task DeleteWorker(int id)
        {
            try
            {
                var worker = await _context.Worker.FindAsync(id);
                if (worker == null)
                {
                    throw new Exception("Worker not found");
                }

                _context.Worker.Remove(worker);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting worker");
                throw;
            }
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Worker
{
    public class WorkerRegister
    {
        [StringLength(100)]
        public required string WorkerName { get; set; }

        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(300)]
        public required string Email { get; set; }

        [StringLength(300)]
        public required string Password { get; set; }

        public required bool IsAdmin { get; set; }
    }
}

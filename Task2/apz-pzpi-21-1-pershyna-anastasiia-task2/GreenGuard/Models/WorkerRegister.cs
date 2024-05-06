using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models
{
    public class WorkerRegister
    {
        [Required]
        [StringLength(100)]
        public required string WorkerName { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(300)]
        public required string Email { get; set; }

        [Required]
        [StringLength(300)]
        public required string Password { get; set; }

        [Required]
        public required bool IsAdmin { get; set; }
    }
}

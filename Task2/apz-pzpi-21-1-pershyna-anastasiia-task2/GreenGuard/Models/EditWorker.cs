using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models
{
    public class EditWorker
    {
        [Required]
        [StringLength(50)]
        public required string WorkerName { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(300)]
        public required string Email { get; set; }

        public string? WorkHours { get; set; }

        public float? HourlyRate { get; set; }

        [Required]
        [StringLength(300)]
        public required string PasswordHash { get; set; }
    }
}

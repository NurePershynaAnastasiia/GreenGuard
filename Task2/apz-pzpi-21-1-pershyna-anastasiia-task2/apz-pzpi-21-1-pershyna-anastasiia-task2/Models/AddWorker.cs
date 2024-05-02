using System.ComponentModel.DataAnnotations;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models
{
    public class AddWorker
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

        [Required]
        public required bool IsAdmin { get; set; }
    }
}

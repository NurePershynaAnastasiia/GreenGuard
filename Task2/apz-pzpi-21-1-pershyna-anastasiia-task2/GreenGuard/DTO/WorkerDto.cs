using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.DTO
{
    public class WorkerDto
    {
        public int WorkerId { get; set; }

        public required string WorkerName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public DateTime? StartWorkTime { get; set; }

        public DateTime? EndWorkTime { get; set; }

        public string? PasswordHash { get; set; }

        public bool? IsAdmin { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.DTO
{
    public class WorkerDto
    {
        public int WorkerId { get; set; }

        public required string WorkerName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? WorkHours { get; set; }

        public float? HourlyRate { get; set; }

        public string? PasswordHash { get; set; }

        public bool? IsAdmin { get; set; }
    }
}

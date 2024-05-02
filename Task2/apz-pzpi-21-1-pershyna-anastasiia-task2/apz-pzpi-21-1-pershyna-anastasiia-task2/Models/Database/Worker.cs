using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database
{
    public class Worker
    {
        [Key]
        [Column("Worker_id")]
        public int WorkerId { get; set; }

        [Column("Worker_name")]
        public required string WorkerName { get; set; }

        [Column("Phone_number")]
        public string? PhoneNumber { get; set; }

        [Column("Email")]
        public string? Email { get; set; }

        [Column("Work_hours")]
        public string? WorkHours { get; set; }

        [Column("Hourly_rate")]
        public float? HourlyRate { get; set; }

        [Column("Password_hash")]
        public string? PasswordHash { get; set; }

        [Column("Is_Admin")]
        public bool? IsAdmin { get; set; }
    }
}

using GreenGuard.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GreenGuard.Dto
{
    public class WorkerDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Worker_id")]
        public int WorkerId { get; set; }

        [Required]
        [Column("Worker_name")]
        public string WorkerName { get; set; }

        [Phone]
        [Column("Phone_number")]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [Column("Email")]
        public string? Email { get; set; }

        [Column("Start_work_time", TypeName = "time(7)")]
        public TimeOnly? StartWorkTime { get; set; }

        [Column("End_work_time", TypeName = "time(7)")]
        public TimeOnly? EndWorkTime { get; set; }

        [Column("Password_hash")]
        public string? PasswordHash { get; set; }

        [Column("Is_Admin")]
        public bool? IsAdmin { get; set; }
    }
}

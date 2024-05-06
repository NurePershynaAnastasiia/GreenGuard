using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database
{
    public class Worker
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Worker_id")]
        public int WorkerId { get; set; }

        [Required]
        [Column("Worker_name")]
        public string WorkerName { get; set; }

        [Column("Phone_number")]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [Column("Email")]
        public string? Email { get; set; }

        [Column("Start_work_time")]
        public DateTime? StartWorkTime { get; set; }

        [Column("End_work_time")]
        public DateTime? EndWorkTime { get; set; }

        [Column("Password_hash")]
        public string? PasswordHash { get; set; }

        [Column("Is_Admin")]
        public bool? IsAdmin { get; set; }
    }
}

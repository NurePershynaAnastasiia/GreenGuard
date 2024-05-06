using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenGuard.Models.Database
{
    public class Task
    {
        [Key]
        [Column("Task_id")]
        public int TaskId { get; set; }

        [Column("Task_date", TypeName = "date")]
        public DateTime TaskDate { get; set; }

        [Column("Task_type")]
        public string? TaskType { get; set; }

        [ForeignKey("Fertilizer_id")]
        public int FertilizerId { get; set; }

        [Column("Task_details")]
        public string? TaskDetails { get; set; }

        [Column("Task_state")]
        public string? TaskState { get; set; }
    }
}

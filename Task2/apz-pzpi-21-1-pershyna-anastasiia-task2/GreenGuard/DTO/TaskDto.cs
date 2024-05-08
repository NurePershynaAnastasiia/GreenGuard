using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenGuard.Dto
{
    public class TaskDto
    {
        [Key]
        [Column("Task_id")]
        public int TaskId { get; set; }

        [Column("Task_date")]
        public DateTime TaskDate { get; set; }

        [Column("Task_type")]
        public string? TaskType { get; set; }

        [ForeignKey("Fertilizer")]
        [Column("Fertilizer_id")]
        public int? FertilizerId { get; set; }

        [Column("Task_details")]
        public string? TaskDetails { get; set; }

        [Column("Task_state")]
        public string? TaskState { get; set; }
    }
}

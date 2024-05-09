using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Dto
{
    public class WorkerInTaskDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Worker_in_Task_id")]
        public int WorkerInTaskId { get; set; }

        [Column("Worker_id")]
        public int WorkerId { get; set; }

        [Column("Task_id")]
        public int TaskId { get; set; }

        [Column("Task_status")]
        public string? TaskStatus { get; set; }
    }
}

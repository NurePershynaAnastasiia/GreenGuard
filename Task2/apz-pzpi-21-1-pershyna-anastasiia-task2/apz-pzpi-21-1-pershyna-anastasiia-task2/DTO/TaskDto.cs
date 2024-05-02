using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.DTO
{
    public class TaskDto
    {
        public int TaskId { get; set; }

        public DateTime TaskDate { get; set; }

        public string? TaskType { get; set; }

        public int FertilizerId { get; set; }

        public string? TaskDetails { get; set; }

        public string? TaskState { get; set; }
    }
}

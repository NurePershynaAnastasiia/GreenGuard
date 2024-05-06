using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models
{
    public class AddTask
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TaskDate { get; set; }

        [StringLength(50)]
        public string? TaskType { get; set; }

        public int? FertilizerId { get; set; }

        [StringLength(500)]
        public string? TaskDetails { get; set; }

        [StringLength(50)]
        public string? TaskState { get; set; }
    }
}

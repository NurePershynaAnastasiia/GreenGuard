using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Database
{
    public class PlantInTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Plant_in_Task_id")]
        public int PlantInTaskId { get; set; }

        [Column("Plant_id")]
        public int? PlantId { get; set; }

        [Column("Task_id")]
        public int? TaskId { get; set; }
    }
}

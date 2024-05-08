using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenGuard.Dto
{
    public class WorkingScheduleDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Working_Schedule_id")]
        public int WorkeingScheduleId { get; set; }

        [Column("Worker_id")]
        public int? WorkerId { get; set; }

        [Column("Monday")]
        public bool? Monday { get; set; }

        [Column("Tuesday")]
        public bool? Tuesday { get; set; }

        [Column("Wednesday")]
        public bool? Wednesday { get; set; }

        [Column("Thursday")]
        public bool? Thursday { get; set; }

        [Column("Friday")]
        public bool? Friday { get; set; }

        [Column("Saturday")]
        public bool? Saturday { get; set; }

        [Column("Sunday")]
        public bool? Sunday { get; set; }
    }
}

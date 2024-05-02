using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database
{
    public class Reminder
    {
        [Key]
        [Column("Reminder_id")]
        public int ReminderId { get; set; }

        [Column("Reminder_date", TypeName = "datetime")]
        public DateTime ReminderDate { get; set; }

        [ForeignKey("Worker_id")]
        public int WorkerId { get; set; }

        [ForeignKey("Task_id")]
        public int TaskId { get; set; }
    }
}

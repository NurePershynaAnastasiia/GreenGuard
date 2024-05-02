using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.DTO
{
    public class ReminderDto
    {
        public int ReminderId { get; set; }

        public DateTime ReminderDate { get; set; }

        public int WorkerId { get; set; }

        public int TaskId { get; set; }
    }
}

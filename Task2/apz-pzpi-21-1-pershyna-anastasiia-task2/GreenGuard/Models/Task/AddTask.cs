using System.ComponentModel.DataAnnotations;
using System.Globalization;
using static GreenGuard.Helpers.DateValidation;

namespace GreenGuard.Models.Task
{
    public class AddTask
    {
        [PastDate(ErrorMessage = "Дата завдання не може бути раніше поточної дати")]
        [ValidDateFormat(ErrorMessage = "Невірний формат дати")]
        public DateTime TaskDate { get; set; }

        public string TaskDetails { get; set; }

        public string TaskType { get; set; }

        public string TaskState { get; set; }

        public int? FertilizerId { get; set; }
    }
}

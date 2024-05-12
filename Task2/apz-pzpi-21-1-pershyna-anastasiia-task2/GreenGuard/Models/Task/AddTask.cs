using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Task
{
    public class AddTask
    {
        [PastDate(ErrorMessage = "Дата завдання не може бути раніше поточної дати")]
        public DateTime TaskDate { get; set; }

        public string TaskDetails { get; set; }

        public string TaskType { get; set; }

        public string TaskState { get; set; }

        public int? FertilizerId { get; set; }
    }

    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = (DateTime)value;
            if (date.Date > DateTime.Now.Date)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessage);
            }
        }
    }
}

using static GreenGuard.Helpers.DateValidation;

namespace GreenGuard.Models.Worker
{
    public class WorkersAtDate
    {
        [ValidDateFormat(ErrorMessage = "Невірний формат дати")]
        public DateTime InputDate { get; set; }
    }
}

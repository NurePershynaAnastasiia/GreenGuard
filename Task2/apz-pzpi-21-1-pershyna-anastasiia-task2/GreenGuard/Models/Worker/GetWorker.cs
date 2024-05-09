using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Worker
{
    public class GetWorker
    {
        public int WorkerId { get; set; }

        public string WorkerName { get; set; }

        [Column("Phone_number")]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public TimeOnly? StartWorkTime { get; set; }

        public TimeOnly? EndWorkTime { get; set; }

        public bool? IsAdmin { get; set; }
    }
}

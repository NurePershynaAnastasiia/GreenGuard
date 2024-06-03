using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using GreenGuard.Helpers;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;

namespace GreenGuard.Models.Worker
{
    public class UpdateWorker
    {
        public string WorkerName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public TimeOnly StartWorkTime { get; set; }

        public TimeOnly EndWorkTime { get; set; }

        public bool isAdmin { get; set; }
    }
}

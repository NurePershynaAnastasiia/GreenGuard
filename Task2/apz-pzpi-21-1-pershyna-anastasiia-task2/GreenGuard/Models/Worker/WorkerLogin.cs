using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Worker
{
    public class WorkerLogin
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}

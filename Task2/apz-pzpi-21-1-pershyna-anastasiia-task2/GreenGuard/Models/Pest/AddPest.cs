using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Pest
{
    public class AddPest
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public required string PestName { get; set; }

        [StringLength(300)]
        public string? PestDescription { get; set; }
    }
}

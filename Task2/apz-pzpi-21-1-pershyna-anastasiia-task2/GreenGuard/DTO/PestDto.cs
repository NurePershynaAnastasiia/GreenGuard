using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.DTO
{
    public class PestDto
    {
        public int PestId { get; set; }

        public required string PestName { get; set; }

        public string? PestDescription { get; set; }
    }
}

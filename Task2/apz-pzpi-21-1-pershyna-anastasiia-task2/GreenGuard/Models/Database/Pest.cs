using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenGuard.Models.Database
{
    public class Pest
    {
        [Key]
        [Column("Pest_id")]
        public int PestId { get; set; }

        [Column("Pest_name")]
        public required string PestName { get; set; }

        [Column("Pest_description")]
        public string? PestDescription { get; set; }
    }
}

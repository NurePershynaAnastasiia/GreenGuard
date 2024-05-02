using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database
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

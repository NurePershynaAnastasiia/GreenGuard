using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenGuard.Models.Database
{
    public class Fertilizer
    {
        [Key]
        [Column("Fertilizer_id")]
        public int FertilizerId { get; set; }

        [Column("Fertilizer_name")]
        public required string FertilizerName { get; set; }

        [Column("Fertilizer_quantity")]
        public int? FertilizerQuantity { get; set; }
    }
}

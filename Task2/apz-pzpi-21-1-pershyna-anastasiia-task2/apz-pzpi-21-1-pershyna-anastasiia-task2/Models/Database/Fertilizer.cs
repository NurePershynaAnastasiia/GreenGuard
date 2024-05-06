using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database
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

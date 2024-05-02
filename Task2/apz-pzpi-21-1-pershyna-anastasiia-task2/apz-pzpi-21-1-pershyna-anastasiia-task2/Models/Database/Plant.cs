using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database
{
    public class Plant
    {
        [Key]
        [Column("Plant_id")]
        public int PlantId { get; set; }

        [Column("Plant_type_id")]
        public int PlantTypeId { get; set; }

        [Column("Plant_name")]
        public required string PlantName { get; set; }

        [Column("Temp")]
        public float? Temp { get; set; }

        [Column("Humidity")]
        public float? Humidity { get; set; }

        [Column("Light")]
        public float? Light { get; set; }

        [Column("Additional_info")]
        public string? AdditionalInfo { get; set; }

        [Column("Plant_state")]
        public string? PlantState { get; set; }
    }
}

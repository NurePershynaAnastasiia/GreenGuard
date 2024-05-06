using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database
{
    public class PlantType
    {
        [Key]
        [Column("Plant_type_id")]
        public int PlantTypeId { get; set; }

        [Column("Plant_type_name")]
        public required string PlantTypeName { get; set; }

        [Column("Water_freq")]
        public int WaterFreq { get; set; }

        [Column("Opt_temp")]
        public float? OptTemp { get; set; }

        [Column("Opt_humidity")]
        public float? OptHumidity { get; set; }

        [Column("Opt_light")]
        public float? OptLight { get; set; }

        [Column("Plant_type_description")]
        public string? PlantTypeDescription { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenGuard.Dto
{
    public class PlantTypeDto
    {
        [Key]
        [Column("Plant_type_id")]
        public int PlantTypeId { get; set; }

        [Column("Plant_type_name")]
        public required string PlantTypeName { get; set; }

        [Column("Water_freq")]
        public int WaterFreq { get; set; }

        [Column("Opt_temp")]
        public double? OptTemp { get; set; }

        [Column("Opt_humidity")]
        public double? OptHumidity { get; set; }

        [Column("Opt_light")]
        public double? OptLight { get; set; }

        [Column("Plant_type_description")]
        public string? PlantTypeDescription { get; set; }
    }
}

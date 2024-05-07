using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenGuard.Models.Database
{
    public class Plant
    {
        [Key]
        [Column("Plant_id")]
        public int PlantId { get; set; }

        [Required]
        [Column("Plant_type_id")]
        public required int PlantTypeId { get; set; }

        [Required]
        [Column("Plant_location")]
        public required string PlantLocation { get; set; }

        [Column("Temp")]
        public double? Temp { get; set; }

        [Column("Humidity")]
        public double? Humidity { get; set; }

        [Column("Light")]
        public double? Light { get; set; }

        [Column("Additional_info")]
        public string? AdditionalInfo { get; set; }

        [Column("Plant_state")]
        public string? PlantState { get; set; }
    }
}

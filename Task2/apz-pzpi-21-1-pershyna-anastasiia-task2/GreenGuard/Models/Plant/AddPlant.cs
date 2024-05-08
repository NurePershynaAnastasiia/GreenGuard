using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Plant
{
    public class AddPlant
    {
        [Required]
        [Column("Plant_type_id")]
        public required int PlantTypeId { get; set; }

        [Required]
        [Column("Plant_location")]
        public required string PlantLocation { get; set; }

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

using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.PlantType
{
    public class AddPlantType
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string PlantTypeName { get; set; }

        public int WaterFreq { get; set; }

        public float? OptTemp { get; set; }

        public float? OptHumidity { get; set; }

        public float? OptLight { get; set; }

        [StringLength(300)]
        public string? PlantTypeDescription { get; set; }
    }
}

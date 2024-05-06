using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models
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

        public int FertilizerId { get; set; }

        [StringLength(300)]
        public string? PlantTypeDescription { get; set; }
    }
}

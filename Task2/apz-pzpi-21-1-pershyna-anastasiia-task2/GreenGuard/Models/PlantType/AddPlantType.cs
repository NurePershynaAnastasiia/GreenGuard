using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.PlantType
{
    public class AddPlantType
    {
        [StringLength(50, MinimumLength = 3)]
        public required string PlantTypeName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Частота поливу має бути додатнім числом.")]
        public int WaterFreq { get; set; }

        [Range(0, 50, ErrorMessage = "Температура має бути додатнім числом.")]
        public float? OptTemp { get; set; }

        [Range(1, 100, ErrorMessage = "Вологість має бути додатнім числом від 1 до 100.")]
        public float? OptHumidity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Освітлення має бути додатнім числом.")]
        public float? OptLight { get; set; }

        [StringLength(300)]
        public string? PlantTypeDescription { get; set; }
    }
}

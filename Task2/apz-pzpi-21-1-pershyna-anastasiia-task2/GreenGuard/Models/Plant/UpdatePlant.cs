using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Plant
{
    public class UpdatePlant
    {
        public required string PlantLocation { get; set; }

        public double? Temp { get; set; }

        public double? Humidity { get; set; }

        public double? Light { get; set; }

        public string? AdditionalInfo { get; set; }

        public string? PlantState { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenGuard.Models.Plant
{
    public class PlantFull
    {
        public required int PlantId { get; set; } 

        public string PlantTypeName { get; set; }

        public required string PlantLocation { get; set; }

        public double? Temp { get; set; }

        public double? Humidity { get; set; }

        public double? Light { get; set; }

        public string? AdditionalInfo { get; set; }

        public string? PlantState { get; set; }

        public List<string>? Pests { get; set; }
    }
}

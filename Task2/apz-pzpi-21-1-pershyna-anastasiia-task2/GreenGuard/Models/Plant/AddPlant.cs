using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Plant
{
    public class AddPlant
    {
        public required int PlantTypeId { get; set; }

        public required string PlantLocation { get; set; }

        public double? Temp { get; set; }

        public double? Humidity { get; set; }

        public double? Light { get; set; }

        public string? AdditionalInfo { get; set; }
    }
}

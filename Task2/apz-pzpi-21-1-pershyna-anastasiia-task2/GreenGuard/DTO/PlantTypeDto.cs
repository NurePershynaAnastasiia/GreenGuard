using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.DTO
{
    public class PlantTypeDto
    {
        public int PlantTypeId { get; set; }

        public required string PlantTypeName { get; set; }

        public int WaterFreq { get; set; }

        public double? OptTemp { get; set; }

        public double? OptHumidity { get; set; }

        public double? OptLight { get; set; }

        public string? PlantTypeDescription { get; set; }
    }
}

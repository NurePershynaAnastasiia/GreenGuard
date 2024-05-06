using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.DTO
{
    public class PlantTypeDto
    {
        public int PlantTypeId { get; set; }

        public required string PlantTypeName { get; set; }

        public int WaterFreq { get; set; }

        public float? OptTemp { get; set; }

        public float? OptHumidity { get; set; }

        public float? OptLight { get; set; }

        public string? PlantTypeDescription { get; set; }
    }
}

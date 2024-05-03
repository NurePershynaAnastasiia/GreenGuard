﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.DTO
{
    public class PlantDto
    {
        public int PlantId { get; set; }

        public required string PlantLocation { get; set; }

        public required int PlantTypeId { get; set; }

        public float? Temp { get; set; }

        public float? Humidity { get; set; }

        public float? Light { get; set; }

        public string? AdditionalInfo { get; set; }

        public required string PlantState { get; set; }
    }
}

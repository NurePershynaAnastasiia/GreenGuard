using System.ComponentModel.DataAnnotations.Schema;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models
{
    public class UpdatePlantState
    {
        [Column("Plant_state")]
        public string? PlantState { get; set; }
    }
}

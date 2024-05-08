using System.ComponentModel.DataAnnotations.Schema;

namespace GreenGuard.Models.Plant
{
    public class UpdatePlantState
    {
        [Column("Plant_state")]
        public string? PlantState { get; set; }
    }
}

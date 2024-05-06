using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.DTO
{
    public class FertilizerDto
    {
        public int FertilizerId { get; set; }

        public required string FertilizerName { get; set; }

        public int? FertilizerQuantity { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models
{
    public class AddFertilizer
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public required string FertilizerName { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Кількість має бути додатнім числом.")]
        public int FertilizerQuantity { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Fertilizer
{
    public class AddFertilizer
    {
        [StringLength(50, MinimumLength = 2)]
        public required string FertilizerName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Кількість має бути додатнім числом.")]
        public int FertilizerQuantity { get; set; }
    }
}

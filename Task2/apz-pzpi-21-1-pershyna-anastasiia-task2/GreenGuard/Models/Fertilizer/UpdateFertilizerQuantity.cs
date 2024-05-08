using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Fertilizer
{
    public class UpdateFertilizerQuantity
    {
        [Range(0, int.MaxValue, ErrorMessage = "Кількість має бути додатнім числом.")]
        public int FertilizerQuantity { get; set; }
    }
}

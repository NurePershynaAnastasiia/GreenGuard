using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.DTO
{
    public class FertilizerDto
    {
        public int FertilizerId { get; set; }

        public required string FertilizerName { get; set; }

        public int? FertilizerQuantity { get; set; }
    }
}

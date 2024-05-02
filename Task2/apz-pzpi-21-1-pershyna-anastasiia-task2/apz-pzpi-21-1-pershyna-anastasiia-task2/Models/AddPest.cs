using System.ComponentModel.DataAnnotations;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Models
{
    public class AddPest
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public required string PestName { get; set; }

        [StringLength(300)]
        public string? PestDescription { get; set; }
    }
}

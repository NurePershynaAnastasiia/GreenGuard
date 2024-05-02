using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.DTO
{
    public class PestDto
    {
        public int PestId { get; set; }

        public required string PestName { get; set; }

        public string? PestDescription { get; set; }
    }
}

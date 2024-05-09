using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Dto
{
    public class PestInPlantDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Pest_in_Plant_id")]
        public int PestInPlantId { get; set; }

        [ForeignKey("Plant")]
        [Column("Plant_id")]
        public required int PlantId { get; set; }

        [ForeignKey("Pest")]
        [Column("Pest_id")]
        public required int PestId { get; set; }
    }
}

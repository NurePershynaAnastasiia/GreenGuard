using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenGuard.Dto
{
    public class PlantDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Plant_id")]
        public int PlantId { get; set; }

        [Required]
        [Column("Plant_type_id")]
        public required int PlantTypeId { get; set; }

        [Required]
        [Column("Plant_location")]
        [StringLength(500, MinimumLength = 3)]
        public required string PlantLocation { get; set; }

        [Column("Temp")]
        public double? Temp { get; set; }

        [Column("Humidity")]
        public double? Humidity { get; set; }

        [Column("Light")]
        public double? Light { get; set; }

        [Column("Additional_info")]
        [StringLength(300)]
        public string? AdditionalInfo { get; set; }

        [Column("Plant_state")]
        [StringLength(50)]
        public string? PlantState { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licensing.Skus
{
    [Table("skus", Schema = "public")]
    public class Sku
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; } = -1;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.MinValue;

        [Required]
        [Column("sku")]
        [StringLength(255)] // Adjust max length if needed
        public required string SkuCode { get; set; }

        [Required]
        [Column("name")]
        [StringLength(255)] // Adjust max length if needed
        public required string Name { get; set; }

        [Column("description")]
        [StringLength(255)] // Adjust max length if needed
        public string? Description { get; set; }
    }
}

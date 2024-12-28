using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licensing.Skus
{
    [Table("skus", Schema = "public")]
    public class SkuEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [StringLength(255)]
        public required string Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; private set; }

        [Required]
        [Column("name")]
        [StringLength(255)] // Adjust max length if needed
        public required string Name { get; set; }

        [Column("description")]
        [StringLength(255)] // Adjust max length if needed
        public string? Description { get; set; }
    }

    public class SkuUpdate
    {
        [Column("name")]
        [StringLength(255)] // Adjust max length if needed
        public required string Name { get; set; }

        [Column("description")]
        [StringLength(255)] // Adjust max length if needed
        public string? Description { get; set; }
    }
}

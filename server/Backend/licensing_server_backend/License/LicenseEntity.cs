using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licensing.License
{
    [Table("licenses", Schema = "public")]
    public class LicenseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public required string Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; private set; }

        [Required]
        [Column("label")]
        [StringLength(255)] // Adjust max length if needed
        public required string? Label { get; set; }

        [Required]
        [Column("issued_by")]
        [StringLength(255)] // Adjust max length if needed
        public required string IssuedBy { get; set; }

        [Required]
        [Column("license")]
        public required string License { get; set; }

        [Column("description")]
        [StringLength(20480)] // Adjust max length if needed
        public string? Description { get; set; }

        [Required]
        [Column("customer_id")]
        [StringLength(255)] // Adjust max length if needed
        public required string CustomerId { get; set; }
    }
}

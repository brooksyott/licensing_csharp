using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Licensing.Customers
{
    [Table("customers", Schema = "public")]
    public class CustomerEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; private set; }

        [Required]
        [Column("name")]
        [StringLength(255)] // Adjust max length if needed
        public required string? Name { get; set; }

        [Column("description")]
        [StringLength(20480)] // Adjust max length if needed
        public string? Description { get; set; }

        [Column("is_visible")]
        public bool Visibility { get; set; } = false;
    }
}

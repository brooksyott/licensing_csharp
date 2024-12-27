using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Licensing.auth
{

    [Table("int_auth_keys", Schema = "public")]
    public class InternalAuthKeyEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("key")]
        public string Key { get; set; } = Guid.NewGuid().ToString();

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; private set; }

        [Required]
        [Column("created_by")]
        [StringLength(255)] // Adjust max length if needed
        public required string? CreatedBy { get; set; }

        [Required]
        [Column("role")]
        [StringLength(255)] // Adjust max length if needed
        public required string? Role { get; set; }

        [Column("description")]
        [StringLength(20480)] // Adjust max length if needed
        public string? Description { get; set; }
    }
}

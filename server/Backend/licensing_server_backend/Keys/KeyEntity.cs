using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Licensing.Keys
{
    [Table("keys", Schema = "public")]
    public class KeyEntity
    {
        // Used to redact sensitive information from the response, specifically the private key
        public const string Redact = "****** REDACTED ******";

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
        [Column("created_by")]
        [StringLength(255)] // Adjust max length if needed
        public required string? CreatedBy { get; set; }

        [Required]
        [Column("updated_by")]
        [StringLength(255)] // Adjust max length if needed
        public required string? UpdatedBy { get; set; }

        [Required]
        [Column("private_key")]
        [StringLength(20480)] // Adjust max length if needed
        public required string? PrivateKey { get; set; }

        [Required]
        [Column("public_key")]
        [StringLength(20480)] // Adjust max length if needed
        public required string? PublicKey { get; set; }

        [Column("description")]
        [StringLength(20480)] // Adjust max length if needed
        public string? Description { get; set; }

    }
}

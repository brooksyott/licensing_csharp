using Licensing.Skus;
using Microsoft.EntityFrameworkCore;

namespace Licensing.Data
{
    public class LicensingContext : DbContext
    {
        public LicensingContext(DbContextOptions<LicensingContext> options) : base(options)
        {
        }

        public DbSet<Sku> Skus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sku>(entity =>
            {
                entity.ToTable("skus", "public");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd(); // Configures auto-increment
            });

            modelBuilder.Entity<Sku>()
                .HasIndex(s => s.SkuCode)
                .IsUnique();

            modelBuilder.Entity<Sku>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<Sku>().HasData(
                new Sku
                {
                    Id = -1,
                    SkuCode = "SKU-001",
                    Name = "Standard",
                    Description = "Standard license"
                },
                new Sku
                {
                    Id = -2,
                    SkuCode = "SKU-002",
                    Name = "Premium",
                    Description = "Premium license"
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}

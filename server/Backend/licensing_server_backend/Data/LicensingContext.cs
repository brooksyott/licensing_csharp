using Licensing.Keys;
using Licensing.License;
using Licensing.Skus;
using Licensing.Customers;
using Microsoft.EntityFrameworkCore;

namespace Licensing.Data
{
    public class LicensingContext : DbContext
    {
        public LicensingContext(DbContextOptions<LicensingContext> options) : base(options)
        {
        }

        public DbSet<Sku> Skus { get; set; }
        public DbSet<KeyEntity> Keys { get; set; }
        public DbSet<LicenseEntity> Licenses { get; set; }
        public DbSet<CustomerEntity> Customers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingSkus(modelBuilder);
            OnModelCreatingCerts(modelBuilder);
            OnModelCreatingLicenses(modelBuilder);
            OnModelCreatingCustomers(modelBuilder);
        }

        protected void OnModelCreatingCustomers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerEntity>(entity =>
            {
                entity.ToTable("customers", "public");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<CustomerEntity>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedAt)
                      .HasColumnName("updated_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAddOrUpdate();
            });
        }

        protected void OnModelCreatingLicenses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LicenseEntity>(entity =>
            {
                entity.ToTable("licenses", "public");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<LicenseEntity>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedAt)
                      .HasColumnName("updated_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAddOrUpdate();
            });
        }

        protected void OnModelCreatingCerts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KeyEntity>(entity =>
            {
                entity.ToTable("keys", "public");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<KeyEntity>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedAt)
                      .HasColumnName("updated_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAddOrUpdate();
            });
        }

        protected void OnModelCreatingSkus(ModelBuilder modelBuilder)
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

            modelBuilder.Entity<Sku>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedAt)
                      .HasColumnName("updated_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAddOrUpdate();
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}

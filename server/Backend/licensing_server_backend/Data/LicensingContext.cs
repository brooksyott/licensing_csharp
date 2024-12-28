using Licensing.Keys;
using Licensing.License;
using Licensing.Skus;
using Licensing.Customers;
using Microsoft.EntityFrameworkCore;
using Licensing.auth;

namespace Licensing.Data
{
    public class LicensingContext : DbContext
    {
        public LicensingContext(DbContextOptions<LicensingContext> options) : base(options)
        {
        }

        public DbSet<SkuEntity> Skus { get; set; }
        public DbSet<KeyEntity> Keys { get; set; }
        public DbSet<LicenseEntity> Licenses { get; set; }
        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<InternalAuthKeyEntity> InternalAuthKeys { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingSkus(modelBuilder);
            OnModelCreatingCerts(modelBuilder);
            OnModelCreatingLicenses(modelBuilder);
            OnModelCreatingCustomers(modelBuilder);
            OnModelCreatingInternalAuthKeys(modelBuilder);
        }

        protected void OnModelCreatingInternalAuthKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InternalAuthKeyEntity>(entity =>
            {
                entity.ToTable("int_auth_keys", "public");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<InternalAuthKeyEntity>(entity =>
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
            modelBuilder.Entity<SkuEntity>(entity =>
            {
                entity.ToTable("skus", "public");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<SkuEntity>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<SkuEntity>(entity =>
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

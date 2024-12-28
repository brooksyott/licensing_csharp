using Licensing.Keys;
using Licensing.License;
using Licensing.Skus;
using Licensing.Customers;
using Microsoft.EntityFrameworkCore;
using Licensing.auth;

namespace Licensing.Data
{
    /// <summary>
    /// The database context for the licensing server.
    /// </summary>
    public class LicensingContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicensingContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public LicensingContext(DbContextOptions<LicensingContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the Skus.
        /// </summary>
        public DbSet<SkuEntity> Skus { get; set; }

        /// <summary>
        /// Gets or sets the Keys.
        /// </summary>
        public DbSet<KeyEntity> Keys { get; set; }

        /// <summary>
        /// Gets or sets the Licenses.
        /// </summary>
        public DbSet<LicenseEntity> Licenses { get; set; }

        /// <summary>
        /// Gets or sets the Customers.
        /// </summary>
        public DbSet<CustomerEntity> Customers { get; set; }

        /// <summary>
        /// Gets or sets the InternalAuthKeys.
        /// </summary>
        public DbSet<InternalAuthKeyEntity> InternalAuthKeys { get; set; }

        /// <summary>
        /// Configures the schema needed for the context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingSkus(modelBuilder);
            OnModelCreatingCerts(modelBuilder);
            OnModelCreatingLicenses(modelBuilder);
            OnModelCreatingCustomers(modelBuilder);
            OnModelCreatingInternalAuthKeys(modelBuilder);
        }

        /// <summary>
        /// Configures the schema for the InternalAuthKeys entity.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
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

        /// <summary>
        /// Configures the schema for the Customers entity.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
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

        /// <summary>
        /// Configures the schema for the Licenses entity.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
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

        /// <summary>
        /// Configures the schema for the Keys entity.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
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

        /// <summary>
        /// Configures the schema for the Skus entity.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
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

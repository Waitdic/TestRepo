using iVectorOne_Admin_Api.Config.Models;
using Microsoft.EntityFrameworkCore;
using Attribute = iVectorOne_Admin_Api.Config.Models.Attribute;

namespace iVectorOne_Admin_Api.Config.Context
{
    public partial class ConfigContext : DbContext
    {
        public ConfigContext()
        {
        }

        public ConfigContext(DbContextOptions<ConfigContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attribute> Attributes { get; set; } = null!;
        public virtual DbSet<Subscription> Subscriptions { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;
        public virtual DbSet<SupplierAttribute> SupplierAttributes { get; set; } = null!;
        public virtual DbSet<SupplierSubscription> SupplierSubscriptions { get; set; } = null!;
        public virtual DbSet<SupplierSubscriptionAttribute> SupplierSubscriptionAttributes { get; set; } = null!;
        public virtual DbSet<Tenant> Tenants { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserTenant> UserTenants { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attribute>(entity =>
            {
                entity.HasKey(e => e.AttributeId)
                    .IsClustered(false);

                entity.ToTable("Attribute");

                entity.HasIndex(e => e.AttributeName, "CK_Unique_AttributeName")
                    .IsUnique();

                entity.Property(e => e.AttributeId).HasColumnName("AttributeID");

                entity.Property(e => e.AttributeName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DefaultValue)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => e.SubscriptionId)
                    .IsClustered(false);

                entity.ToTable("Subscription");

                entity.HasIndex(e => e.Login, "CK_Unique_SubscriptionLogin")
                    .IsUnique();

                entity.HasIndex(e => e.TenantId, "IX_Subscription_TenantID");

                entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Environment)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.Login)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PropertyTprequestLimit).HasColumnName("PropertyTPRequestLimit");

                entity.Property(e => e.TenantId)
                    .HasColumnName("TenantID")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_Subscription_Tenant");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierId)
                    .IsClustered(false);

                entity.ToTable("Supplier");

                entity.HasIndex(e => e.SupplierName, "CK_Unique_SupplierName")
                    .IsUnique();

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.SupplierName).HasMaxLength(200);
            });

            modelBuilder.Entity<SupplierAttribute>(entity =>
            {
                entity.HasKey(e => e.SupplierAttributeId)
                    .HasName("PK_SupplierSubscription")
                    .IsClustered(false);

                entity.ToTable("SupplierAttribute");

                entity.HasIndex(e => e.AttributeId, "IX_SupplierAttribute_AttributeID");

                entity.HasIndex(e => e.SupplierId, "IX_SupplierAttribute_SupplierID");

                entity.HasIndex(e => new { e.SupplierId, e.AttributeId }, "UN_SupplierIDAttributeID")
                    .IsUnique();

                entity.Property(e => e.SupplierAttributeId).HasColumnName("SupplierAttributeID");

                entity.Property(e => e.AttributeId).HasColumnName("AttributeID");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.SupplierAttributes)
                    .HasForeignKey(d => d.AttributeId)
                    .HasConstraintName("FK_Attribute_SupplierAttribute");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.SupplierAttributes)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_Supplier_SupplierAttribute");
            });

            modelBuilder.Entity<SupplierSubscription>(entity =>
            {
                entity.HasKey(e => e.SupplierSubscriptionId)
                    .HasName("PK_SupplierSubscription_1")
                    .IsClustered(false);

                entity.ToTable("SupplierSubscription");

                entity.HasIndex(e => e.SubscriptionId, "IX_SupplierSubscription_SubscriptionID");

                entity.HasIndex(e => e.SupplierId, "IX_SupplierSubscription_SupplierID");

                entity.HasIndex(e => new { e.SupplierId, e.SubscriptionId }, "UN_SupplierIDSubscriptionID")
                    .IsUnique();

                entity.Property(e => e.SupplierSubscriptionId).HasColumnName("SupplierSubscriptionID");

                entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.HasOne(d => d.Subscription)
                    .WithMany(p => p.SupplierSubscriptions)
                    .HasForeignKey(d => d.SubscriptionId)
                    .HasConstraintName("FK_Subscription_SupplierSubscription");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.SupplierSubscriptions)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_Supplier_SupplierSubscription");
            });

            modelBuilder.Entity<SupplierSubscriptionAttribute>(entity =>
            {
                entity.HasKey(e => e.SupplierSubscriptionAttributeId)
                    .IsClustered(false);

                entity.ToTable("SupplierSubscriptionAttribute");

                entity.HasIndex(e => e.SubscriptionId, "IX_SupplierSubscriptionAttribute_SubscriptionID");

                entity.HasIndex(e => e.SupplierAttributeId, "IX_SupplierSubscriptionAttribute_SupplierAttributeID");

                entity.HasIndex(e => new { e.SubscriptionId, e.SupplierAttributeId }, "UN_SubscriptionIDSupplierAttributeID")
                    .IsUnique();

                entity.Property(e => e.SupplierSubscriptionAttributeId).HasColumnName("SupplierSubscriptionAttributeID");

                entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");

                entity.Property(e => e.SupplierAttributeId).HasColumnName("SupplierAttributeID");

                entity.HasOne(d => d.Subscription)
                    .WithMany(p => p.SupplierSubscriptionAttributes)
                    .HasForeignKey(d => d.SubscriptionId)
                    .HasConstraintName("FK_SupplierSubscriptionAttribute_Subscription");

                entity.HasOne(d => d.SupplierAttribute)
                    .WithMany(p => p.SupplierSubscriptionAttributes)
                    .HasForeignKey(d => d.SupplierAttributeId)
                    .HasConstraintName("FK_SupplierSubscriptionAttribute_SupplierAttribute");
            });

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenant");

                entity.Property(e => e.TenantId).HasColumnName("TenantID");

                entity.Property(e => e.CompanyName).HasMaxLength(100);

                entity.Property(e => e.ContactEmail).HasMaxLength(200);

                entity.Property(e => e.ContactName).HasMaxLength(100);

                entity.Property(e => e.ContactTelephone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.TenantKey).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.UserName).HasMaxLength(100);
            });

            modelBuilder.Entity<UserTenant>(entity =>
            {
                entity.ToTable("UserTenant");

                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.UserTenants)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_UserTenant_Tenant");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTenants)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserTenant_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

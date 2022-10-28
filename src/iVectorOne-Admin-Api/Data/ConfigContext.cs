using iVectorOne_Admin_Api.Data.Models;
using iVectorOne_Admin_Api.Data.Models.Dashboard;
using Newtonsoft.Json;
using System.Text.Json;
using Attribute = iVectorOne_Admin_Api.Config.Models.Attribute;

namespace iVectorOne_Admin_Api.Data
{
    public partial class AdminContext : DbContext
    {
        public AdminContext()
        {
        }

        public AdminContext(DbContextOptions<AdminContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Authorisation> Authorisations { get; set; } = null!;
        public virtual DbSet<Attribute> Attributes { get; set; } = null!;
        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;
        public virtual DbSet<SupplierAttribute> SupplierAttributes { get; set; } = null!;
        public virtual DbSet<AccountSupplier> AccountSuppliers { get; set; } = null!;
        public virtual DbSet<AccountSupplierAttribute> AccountSupplierAttributes { get; set; } = null!;
        public virtual DbSet<Tenant> Tenants { get; set; } = null!;
        public virtual DbSet<UserTenant> UserTenants { get; set; } = null!;
        public virtual DbSet<Property> Properties { get; set; } = null!;
        public virtual DbSet<BookingLog> BookingLogs { get; set; } = null!;

        public virtual DbSet<FireForgetSearchResponse> FireForgetSearchResponses { get; set; } = null!;

        //
        // Dashboard
        //

        public virtual DbSet<SearchesByHour> SearchesByHour { get; set; } = null!;

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SearchesByHour>(e =>
            {
                e.HasNoKey();

            });

            modelBuilder.Entity<FireForgetSearchResponse>(e =>
            {
                e.ToTable("FireForgetSearchResponse");
                e.HasKey(e => e.FireForgetSearchResponseId).IsClustered(false);

                e.Property(e => e.SearchResponse)
                 .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => v == null
                         ? null
                         : System.Text.Json.JsonSerializer.Deserialize<iVectorOne.SDK.V2.PropertySearch.Response>(v, new JsonSerializerOptions()));
            });

            modelBuilder.Entity<Property>(e =>
            {
                e.HasNoKey();

            });

            modelBuilder.Entity<BookingLog>(e =>
            {
                //e.HasKey(e => e.BookingId);
                e.HasNoKey();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.UserName).HasMaxLength(100);
            });


            modelBuilder.Entity<Authorisation>(entity =>
            {
                entity.ToTable("Admin_Authorisation");

                entity.HasKey(e => e.AuthorisationId)
                    .IsClustered(false);
            });

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

                entity.Property(e => e.Schema)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccountId)
                    .IsClustered(false);

                entity.ToTable("Account");

                entity.HasIndex(e => e.Login, "CK_Unique_AccountLogin")
                    .IsUnique();

                entity.HasIndex(e => e.TenantId, "FK_Account_TenantID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

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

                entity.Property(e => e.PropertyTpRequestLimit).HasColumnName("PropertyTPRequestLimit");

                entity.Property(e => e.TenantId)
                    .HasColumnName("TenantID")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK_Account_Tenant");

                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'active'")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.EncryptedPassword)
                    .HasMaxLength(500)
                    .IsUnicode(false);
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

                entity.Property(e => e.TestPropertyIDs).HasMaxLength(100);
            });

            modelBuilder.Entity<SupplierAttribute>(entity =>
            {
                entity.HasKey(e => e.SupplierAttributeId)
                    .HasName("PK_AccountSupplier")
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

            modelBuilder.Entity<AccountSupplier>(entity =>
            {
                entity.HasKey(e => e.AccountSupplierId)
                    .HasName("PK_AccountSupplier")
                    .IsClustered(false);

                entity.ToTable("AccountSupplier");

                entity.HasIndex(e => e.AccountId, "IX_AccountSupplier_AccountID");

                entity.HasIndex(e => e.SupplierId, "IX_AccountSupplier_SupplierID");

                entity.HasIndex(e => new { e.SupplierId, e.AccountId }, "UN_AccountIDSupplierID")
                    .IsUnique();

                entity.Property(e => e.AccountSupplierId).HasColumnName("AccountSupplierID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.Enabled).HasDefaultValue(false);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountSuppliers)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Account_AccountSupplier");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.AccountSuppliers)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_Supplier_AccountSupplier");
            });

            modelBuilder.Entity<AccountSupplierAttribute>(entity =>
            {
                entity.HasKey(e => e.AccountSupplierAttributeId)
                    .IsClustered(false);

                entity.ToTable("AccountSupplierAttribute");

                entity.HasIndex(e => e.AccountId, "IX_AccountSupplierAttribute_AccountID");

                entity.HasIndex(e => e.SupplierAttributeId, "IX_AccountSupplierAttribute_SupplierAttributeID");

                entity.HasIndex(e => new { e.AccountId, e.SupplierAttributeId }, "UN_AccountIDSupplierAttributeID")
                    .IsUnique();

                entity.Property(e => e.AccountSupplierAttributeId).HasColumnName("AccountSupplierAttributeID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.SupplierAttributeId).HasColumnName("SupplierAttributeID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountSupplierAttributes)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_AccountSupplierAttribute_Account");

                entity.HasOne(d => d.SupplierAttribute)
                    .WithMany(p => p.AccountSupplierAttributes)
                    .HasForeignKey(d => d.SupplierAttributeId)
                    .HasConstraintName("FK_AccountSupplierAttribute_SupplierAttribute");
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
        }
    }
}

namespace iVectorOne_Admin_Api.Data
{
    public partial class UserTenant
    {
        public int UserTenantId { get; set; }
        public int UserId { get; set; }
        public int TenantId { get; set; }

        public virtual Tenant Tenant { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
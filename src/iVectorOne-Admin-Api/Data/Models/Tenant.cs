namespace iVectorOne_Admin_Api.Data
{
    public partial class Tenant
    {
        public Tenant()
        {
            Subscriptions = new List<Subscription>();
            UserTenants = new List<UserTenant>();
        }

        public int TenantId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? ContactTelephone { get; set; }
        public string? ContactEmail { get; set; }
        public string Status { get; set; } = null!;
        public Guid TenantKey { get; set; }

        public List<Subscription> Subscriptions { get; set; }
        public List<UserTenant> UserTenants { get; set; }
    }
}

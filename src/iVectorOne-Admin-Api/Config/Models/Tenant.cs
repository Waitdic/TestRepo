namespace iVectorOne_Admin_Api.Config.Models
{
    public partial class Tenant
    {
        public Tenant()
        {
            Subscriptions = new HashSet<Subscription>();
        }

        public int TenantId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? ContactTelephone { get; set; }
        public string? ContactEmail { get; set; }
        public string Status { get; set; } = null!;

        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}

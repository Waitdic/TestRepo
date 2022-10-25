namespace iVectorOne_Admin_Api.Security
{
    using System.Security.Principal;

    public class TenantIdentity : GenericIdentity
    {
        public TenantIdentity(Tenant tenant) : base(tenant.CompanyName)
        {
            this.Tenant = tenant;
        }

        public Tenant Tenant { get; set; }
    }
}
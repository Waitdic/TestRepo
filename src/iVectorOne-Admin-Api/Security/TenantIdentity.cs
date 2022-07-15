using iVectorOne_Admin_Api.Config.Models;
using System.Security.Principal;

namespace iVectorOne_Admin_Api.Security
{
    public class TenantIdentity : GenericIdentity
    {
        public TenantIdentity(Tenant tenant) : base(tenant.CompanyName)
        {
            this.Tenant = tenant;
        }

        public Tenant Tenant { get; set; }
    }
}

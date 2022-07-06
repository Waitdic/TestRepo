using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Security
{
    public interface ITenantService
    {
        public Task<Tenant> GetTenant(Guid tenantKey);
    }
}

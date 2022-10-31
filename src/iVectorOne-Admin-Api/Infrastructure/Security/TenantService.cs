namespace iVectorOne_Admin_Api.Security
{
    public class TenantService : ITenantService
    {
        private readonly AdminContext _context;

        public TenantService(AdminContext context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetTenant(Guid tenantKey)
        {
            var tenant = await _context.Tenants.Where(t => t.TenantKey == tenantKey).FirstOrDefaultAsync();
            return tenant;
        }
    }
}
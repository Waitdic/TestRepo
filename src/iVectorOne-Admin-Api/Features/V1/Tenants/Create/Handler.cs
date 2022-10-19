using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Create
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;

        public Handler(AdminContext context)
        {
            _context = context;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var user = await _context.Users
                .Where(u => u.Key == request.UserKey)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (user == null)
            {
                response.BadRequest("User not found.");
                return response;
            }

            var existingTenant = await _context.Tenants
                .Where(u => u.CompanyName == request.CompanyName)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (existingTenant != null)
            {
                response.BadRequest("Tenant already exists.");
                return response;
            }

            var tenant = new Tenant
            {
                CompanyName = request.CompanyName,
                ContactEmail = request.ContactEmail,
                ContactName = request.ContactName,
                ContactTelephone = request.ContactTelephone,
                Status = RecordStatus.Active
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync(cancellationToken);

            _context.UserTenants.Add(new UserTenant { TenantId = tenant.TenantId, UserId = user.UserId });
            _context.Authorisations.Add(new Data.Models.Authorisation
            {
                User = $"userid:{user.UserId}",
                Relationship = "owner",
                Object = $"tenantid:{tenant.TenantId}"
            });
            await _context.SaveChangesAsync(cancellationToken);

            response.Ok(new ResponseModel { Success = true, TenantId = tenant.TenantId });

            return response;
        }
    }
}

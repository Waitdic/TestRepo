using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Users.Link
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

            var tenant = await _context.Tenants
                .Where(t => t.TenantId == request.TenantId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var user = await _context.Users
                .Where(t => t.UserId == request.UserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (tenant == null || user == null)
            {
                response.NotFound("Tenant and / or User not found.");
                return response;
            }

            var userTenant = await _context.UserTenants
                .Where(t => t.TenantId == request.TenantId && t.UserId == request.UserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (userTenant != null)
            {
                response.Ok(new ResponseModel { Success = true, TenantId = request.TenantId });
                return response;
            }

            _context.UserTenants.Add(new UserTenant { TenantId = request.TenantId, UserId = request.UserId });

            _context.Authorisations.Add(new Data.Models.Authorisation
            {
                User = $"userid:{request.UserId}",
                Relationship = request.Relationship.ToLower(),
                Object = $"tenantid:{request.TenantId}"
            });

            await _context.SaveChangesAsync(cancellationToken);

            response.Ok(new ResponseModel { Success = true, TenantId = request.TenantId });

            return response;
        }
    }
}

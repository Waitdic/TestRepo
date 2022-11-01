using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Users.Unlink
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;
        private readonly IMapper _mapper;

        public Handler(AdminContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (userTenant == null)
            {
                response.Ok(new ResponseModel { Success = true, TenantId = request.TenantId });
                return response;
            }

            _context.Remove(userTenant);
            await _context.SaveChangesAsync(cancellationToken);

            var authorisation = await _context.Authorisations
                .Where(a => a.User == $"userid:{request.UserId}" && a.Object == $"tenantid:{request.TenantId}")
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (authorisation == null)
            {
                response.Ok(new ResponseModel { Success = true, TenantId = request.TenantId });
            }
            else
            {
                _context.Remove(authorisation);
                await _context.SaveChangesAsync(cancellationToken);
            }

            response.Ok(new ResponseModel { Success = true, TenantId = request.TenantId });

            return response;
        }
    }
}

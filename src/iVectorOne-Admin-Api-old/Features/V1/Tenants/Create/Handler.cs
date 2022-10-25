using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Create
{
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ConfigContext _context;
        private readonly IMapper _mapper;

        public Handler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var user = await _context.Users.Where(u => u.Key == request.UserKey).FirstOrDefaultAsync();

            if (user == null)
            {
                response.NotFound();
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
            await _context.SaveChangesAsync();

            _context.UserTenants.Add(new UserTenant { TenantId = tenant.TenantId, UserId = user.UserId });
            _context.Authorisations.Add(new Data.Models.Authorisation
            {
                User = $"userid:{user.UserId}",
                Relationship = "owner",
                Object = $"tenantid:{tenant.TenantId}"
            });

            await _context.SaveChangesAsync();

            response.Default(new ResponseModel { Success = true, TenantId = tenant.TenantId });

            return response;
        }
    }
}

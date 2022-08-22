namespace iVectorOne_Admin_Api.Features.V1.Tenants.Users.Link
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

            var userTenant = await _context.UserTenants
                .Where(t => t.TenantId == request.TenantId && t.UserId == request.UserId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (userTenant != null)
            {
                response.Default(new ResponseModel { Success = true, TenantId = request.TenantId });
                return response;
            }

            _context.UserTenants.Add(new UserTenant { TenantId = request.TenantId, UserId = request.UserId });

            _context.Authorisations.Add(new Data.Models.Authorisation
            {
                User = $"userid:{request.UserId}",
                Relationship = request.Relationship.ToLower(),
                Object = $"tenantid:{request.TenantId}"
            });

            await _context.SaveChangesAsync();

            response.Default(new ResponseModel { Success = true, TenantId = request.TenantId });

            return response;
        }
    }
}

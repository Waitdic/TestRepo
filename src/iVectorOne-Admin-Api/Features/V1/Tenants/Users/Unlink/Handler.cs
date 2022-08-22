namespace iVectorOne_Admin_Api.Features.V1.Tenants.Users.Unlink
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
                .FirstOrDefaultAsync();

            if (userTenant == null)
            {
                response.NotFound();
                return response;
            }

            _context.Remove(userTenant);
            await _context.SaveChangesAsync();

            var authorisation = await _context.Authorisations
                .Where(a => a.User == $"userid:{request.UserId}" && a.Object == $"tenantid:{request.TenantId}")
                .FirstOrDefaultAsync();

            if (authorisation == null)
            {
                response.Default(new ResponseModel { Success = true, TenantId = request.TenantId });
            }

            _context.Remove(authorisation);
            await _context.SaveChangesAsync();

            response.Default(new ResponseModel { Success = true, TenantId = request.TenantId });

            return response;
        }
    }
}

namespace iVectorOne_Admin_Api.Features.V1.Users.Info
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

            var user = await _context.Users.Where(t => t.Key == request.Key)
                                            .Include(u => u.UserTenants.Where(t => t.Tenant.Status == "active"))
                                            .ThenInclude(t => t.Tenant)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (user == null)
            {
                response.NotFound();
                return response;
            }

            var tenants = _mapper.Map<List<TenantDto>>(user.UserTenants.Select(x => x.Tenant));

            var authorisations = await _context.Authorisations.Where(a => a.User == $"userid:{user.UserId}")
                                                                .AsNoTracking()
                                                                .ToListAsync();

            var authoriationsDto = authorisations.Select(a => new AuthorisationDto
            {
                Object = a.Object,
                Relationship = a.Relationship,
                User = a.User
            }).ToList();

            response.Default(new ResponseModel
            {
                Success = true,
                UserName = user.UserName,
                Tenants = tenants,
                Authorisations = authoriationsDto
            });

            return response;
        }
    }
}

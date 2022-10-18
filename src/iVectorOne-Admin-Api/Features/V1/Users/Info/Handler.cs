using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Users.Info
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly ConfigContext _context;
        private readonly IMapper _mapper;

        public Handler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var user = await _context.Users.Where(t => t.Key == request.Key)
                                            .Include(u => u.UserTenants.Where(t => t.Tenant.Status == "active"))
                                            .ThenInclude(t => t.Tenant)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (user == null)
            {
                response.NotFound("User not found.");
                return response;
            }

            var tenants = _mapper.Map<List<TenantDto>>(user.UserTenants.Select(x => x.Tenant));

            var authorisations = await _context.Authorisations.Where(a => a.User == $"userid:{user.UserId}")
                                                                .AsNoTracking()
                                                                .ToListAsync(cancellationToken: cancellationToken);

            var authoriationsDto = authorisations.Select(a => new AuthorisationDto
            {
                Object = a.Object,
                Relationship = a.Relationship,
                User = a.User
            }).ToList();

            response.Ok(new ResponseModel
            {
                Success = true,
                UserName = user.UserName,
                UserId = user.UserId,
                Tenants = tenants,
                Authorisations = authoriationsDto
            });

            return response;
        }
    }
}
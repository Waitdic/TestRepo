namespace iVectorOne_Admin_Api.Features.V1.Tenants.Info
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

            var tenantModel = await _context.Tenants.Where(t => t.TenantId == request.TenantId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (tenantModel == null)
            {
                response.NotFound();
                return response;
            }

            var tenant = _mapper.Map<TenantDto>(tenantModel);

            response.Default(new ResponseModel { Success = true, Tenant = tenant });

            return response;
        }
    }
}

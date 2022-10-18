using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Info
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

            var tenant = await _mapper.ProjectTo<TenantDto>(_context.Tenants)
                .Where(t => t.TenantId == request.TenantId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (tenant == null)
            {
                response.NotFound("Tenant not found.");
                return response;
            }

            response.Ok(new ResponseModel { Success = true, Tenant = tenant });
            return response;
        }
    }
}

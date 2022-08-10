using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.List
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

            var tenant = await _context.Tenants
                            .Where(t => t.TenantId == request.TenantId)
                            .Include(t => t.Subscriptions.Where(s => s.Status == RecordStatus.Active)).FirstOrDefaultAsync();

            if (tenant == null)
            {
                response.NotFound();
                return response;
            }

            var subscriptions = _mapper.Map<List<SubscriptionDto>>(tenant.Subscriptions);

            response.Default(new TenantDto { TenantId = tenant.TenantId, Subscriptions = subscriptions, TenantName = tenant.CompanyName });

            return response;
        }
    }
}

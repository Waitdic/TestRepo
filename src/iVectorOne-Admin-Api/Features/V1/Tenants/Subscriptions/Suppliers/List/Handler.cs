namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.List
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

            var subscription = _context.Subscriptions
                .Where(x => x.TenantId == request.TenantId && x.SubscriptionId == request.SubscriptionId)
                .Include(x => x.SupplierSubscriptions)
                .ThenInclude(x => x.Supplier)
                .AsNoTracking()
                .FirstOrDefault();

            if (subscription == null)
            {
                response.NotFound();
                return response;
            }

            var suppliers = _mapper.Map<List<SupplierDto>>(subscription.SupplierSubscriptions);

            response.Default(new SubscriptionDto { SubscriptionId = request.SubscriptionId, SupplierSubscriptions = suppliers});

            return response;
        }
    }
}

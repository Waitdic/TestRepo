namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Info
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

            var subscription  = await _context.Subscriptions
                .Where (x => x.SubscriptionId == request.SubscriptionId && x.TenantId == request.TenantId)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                response.NotFound();
                return response;
            }

            var subscriptionDto = _mapper.Map<SubscriptionDto>(subscription);

            response.Default(subscriptionDto);

            return response;
        }
    }
}

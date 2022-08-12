namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Create
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

            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId).Include(t => t.Subscriptions).FirstOrDefault();

            if (tenant is null)
            {
                response.NotFound();
                return response;
            }

            Subscription CreateSubscription(bool isLive)
            {
                var subscription = _mapper.Map<SubscriptionDto, Subscription>(request.Subscription);
                var environment = isLive ? "Live" : "Test";

                subscription.TenantId = request.TenantId;
                subscription.DummyResponses = false;
                subscription.LogMainSearchError = true;
                subscription.Environment = environment.ToLower();
                subscription.Login = $"{subscription.Login}_{environment}";
                subscription.Password = "123"; // TODO: need to generate
                return subscription;
            }

            var subscriptions = new[]
            {
                CreateSubscription(true),
                CreateSubscription(false)
            };

            if (tenant.Subscriptions.Any(s => subscriptions.Any(n => n.Login == s.Login)))
            {
                response.Warnings.Add("An entry with the same name already exists for this Tenant");

                response.BadRequest();
                return response;
            }

            tenant.Subscriptions.AddRange(subscriptions);
            await _context.SaveChangesAsync(cancellationToken);

            response.Default(new ResponseModelBase { Success = true });
            return response;
        }
    }
}

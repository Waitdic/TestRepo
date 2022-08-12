﻿namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Update
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

            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId)
                .Include(t => t.Subscriptions)
                .FirstOrDefault();

            if (tenant is null)
            {
                response.NotFound();
                return response;
            }

            var subscription = tenant.Subscriptions.FirstOrDefault(s => s.SubscriptionId == request.SubscriptionId);

            if (subscription is null)
            {
                response.NotFound();
                return response;
            }

            _mapper.Map(request.Subscription, subscription);
            await _context.SaveChangesAsync(cancellationToken);

            response.Default();
            return response;
        }
    }
}
namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.Delete
{
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ConfigContext _context;

        public Handler(ConfigContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId)
                .Include(t => t.Subscriptions)
                .ThenInclude(t => t.SupplierSubscriptions)
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

            var supplierSubscription = subscription.SupplierSubscriptions.FirstOrDefault(s => s.SupplierId == request.SupplierId);

            if (supplierSubscription is null)
            {
                response.NotFound();
                return response;
            }

            _context.SupplierSubscriptions.Remove(supplierSubscription);
            await _context.SaveChangesAsync(cancellationToken);

            response.Default();

            return response;
        }
    }
}

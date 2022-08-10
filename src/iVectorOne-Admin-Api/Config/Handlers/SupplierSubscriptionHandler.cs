using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class SupplierSubscriptionHandler : IRequestHandler<SupplierSubscriptionRequest, SupplierSubscriptionResponse>
    {
        private readonly ConfigContext _context;
        private readonly IMapper _mapper;

        public SupplierSubscriptionHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        Task<SupplierSubscriptionResponse> IRequestHandler<SupplierSubscriptionRequest, SupplierSubscriptionResponse>.Handle(SupplierSubscriptionRequest request, CancellationToken cancellationToken)
        {
            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId).Include(t => t.Subscriptions)
                                                                                        .ThenInclude(s => s.SupplierSubscriptions)
                                                                                            .ThenInclude(s => s.Supplier).FirstOrDefault();
            Subscription? subscription = null;
            List<SupplierDTO> suppliers = new List<SupplierDTO>();
            var warnings = new List<string>();
            bool success = false;
            if (tenant != null)
            {
                subscription = tenant.Subscriptions.Where(s => s.SubscriptionId == request.SubscriptionId).FirstOrDefault();
                if (subscription != null)
                {
                    suppliers = _mapper.Map<List<SupplierDTO>>(subscription.SupplierSubscriptions);
                    success = true;
                }
                else
                {
                    warnings.Add(Warnings.ConfigWarnings.NoSubscriptionWarning);
                }
            }
            else
            {
                warnings.Add(Warnings.ConfigWarnings.NoTenantWarning);
            }

            return Task.FromResult(new SupplierSubscriptionResponse() { SupplierSubscriptions = suppliers, Warnings = warnings, Success = success });
        }
    }
}

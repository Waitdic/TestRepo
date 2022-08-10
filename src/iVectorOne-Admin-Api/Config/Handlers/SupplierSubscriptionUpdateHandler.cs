using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class SupplierSubscriptionUpdateHandler : IRequestHandler<SupplierSubscriptionUpdateRequest, SupplierSubscriptionUpdateResponse>
    {
        private readonly ConfigContext _context;

        public SupplierSubscriptionUpdateHandler(ConfigContext context)
        {
            _context = context;
        }

        async Task<SupplierSubscriptionUpdateResponse> IRequestHandler<SupplierSubscriptionUpdateRequest, SupplierSubscriptionUpdateResponse>.Handle(SupplierSubscriptionUpdateRequest request, CancellationToken cancellationToken)
        {
            Subscription? subscription = null;
            List<SupplierDTO> suppliers = new List<SupplierDTO>();
            var warnings = new List<string>();
            bool success = false;

            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId).Include(t => t.Subscriptions.Where(s=> s.SubscriptionId == request.SubscriptionId))
                                                                                        .ThenInclude(s => s.SupplierSubscriptions
                                                                                            .Where(ss => ss.SupplierId == request.SupplierId && ss.SubscriptionId == request.SubscriptionId))
                                                                                            .ThenInclude(s => s.Supplier).FirstOrDefault();

            if (tenant is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoTenantWarning);
            }

            if (!warnings.Any() && tenant?.Subscriptions.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoSubscriptionWarning);
            }

            if (!warnings.Any() && tenant?.Subscriptions.FirstOrDefault()?.SupplierSubscriptions.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoSupplierWarning);
            }

            if (!warnings.Any())
            {
                try
                {
                    var supplierSub = tenant.Subscriptions.FirstOrDefault()?.SupplierSubscriptions.FirstOrDefault()!;
                    supplierSub.Enabled = request.Enabled;
                    await _context.SaveChangesAsync();
                    success = true;
                }
                catch (Exception ex)
                {
                    warnings.Add(ex.Message.ToString());
                }
            }

            return new SupplierSubscriptionUpdateResponse() { Warnings = warnings, Success = success };
        }
    }
}
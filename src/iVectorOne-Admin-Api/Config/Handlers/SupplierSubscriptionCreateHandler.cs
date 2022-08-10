using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class SupplierSubscriptionCreateHandler : IRequestHandler<SupplierSubscriptionCreateRequest, SupplierSubscriptionCreateResponse>
    {
        private readonly ConfigContext _context;

        public SupplierSubscriptionCreateHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
        }

        async Task<SupplierSubscriptionCreateResponse> IRequestHandler<SupplierSubscriptionCreateRequest, SupplierSubscriptionCreateResponse>.Handle(SupplierSubscriptionCreateRequest request, CancellationToken cancellationToken)
        {
            var warnings = new List<string>();
            bool success = false;
            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId)
                                            .Include(t => t.Subscriptions.Where(s => s.SubscriptionId == request.SubscriptionId))
                                            .FirstOrDefault();
            var supplierattributes = _context.SupplierAttributes.Where(sa => sa.SupplierId == request.SupplierId);

            if (tenant is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoTenantWarning);
            }

            if (!warnings.Any() && tenant?.Subscriptions.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoSubscriptionWarning);
            }

            if (!warnings.Any() && supplierattributes.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.SingleNoSupplierAttributeWarning);
            }

            if (!warnings.Any())
            {
                if (_context.SupplierSubscriptions.Any(ss => ss.SupplierId == request.SupplierId && ss.SubscriptionId == request.SubscriptionId))
                {
                    warnings.Add("An entry already exists for this subscription/supplier combination");
                }
                else
                {
                    var supplierSubscription = new SupplierSubscription() { SupplierId = (short)request.SupplierId, SubscriptionId = request.SubscriptionId };
                    _context.SupplierSubscriptions.Add(supplierSubscription);
                }

                var supplierSubscriptionAttributes = new List<SupplierSubscriptionAttribute>();
                foreach (var supplierAttribute in request.SupplierAttributes)
                {
                    try
                    {
                        var attribute = new SupplierSubscriptionAttribute()
                        {
                            SubscriptionId = request.SubscriptionId,
                            SupplierAttributeId = supplierAttribute.SupplierAttributeId,
                            Value = supplierAttribute.Value
                        };

                        supplierSubscriptionAttributes.Add(attribute);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                    }
                }

                _context.SupplierSubscriptionAttributes.AddRange(supplierSubscriptionAttributes);

                if (!warnings.Any() && _context.ChangeTracker.HasChanges())
                {
                    await _context.SaveChangesAsync();
                    success = true;
                }
            }

            return new SupplierSubscriptionCreateResponse() { Warnings = warnings, Success = success };
        }
    }
}
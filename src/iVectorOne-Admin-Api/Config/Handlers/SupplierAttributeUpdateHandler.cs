using AutoMapper;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using iVectorOne_Admin_Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class SupplierAttributeUpdateHandler : IRequestHandler<SupplierAttributeUpdateRequest, SupplierAttributeUpdateResponse>
    {
        private readonly ConfigContext _context;

        public SupplierAttributeUpdateHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
        }

        async Task<SupplierAttributeUpdateResponse> IRequestHandler<SupplierAttributeUpdateRequest, SupplierAttributeUpdateResponse>.Handle(SupplierAttributeUpdateRequest request, CancellationToken cancellationToken)
        {
            var warnings = new List<string>();
            bool success = false;
            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId)
                                            .Include(t => t.Subscriptions.Where(s => s.SubscriptionId == request.SubscriptionId))
                                            .ThenInclude(s => s.SupplierSubscriptionAttributes
                                                            .Where(ssa => ssa.SupplierAttribute.SupplierId == request.SupplierId))
                                            .FirstOrDefault();

            if (tenant is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoTenantWarning);
            }

            if (!warnings.Any() && tenant?.Subscriptions.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoSubscriptionWarning);
            }

            if (!warnings.Any() && tenant?.Subscriptions.FirstOrDefault()?.SupplierSubscriptionAttributes.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.SingleNoSupplierAttributeWarning);
            }

            if (!warnings.Any())
            {
                foreach (var attribute in request.Attributes)
                {
                    try
                    {
                        var supplierSubAttribute = tenant.Subscriptions.FirstOrDefault()?.SupplierSubscriptionAttributes.FirstOrDefault(x => x.SupplierSubscriptionAttributeId == attribute.SupplierSubscriptionAttributeID)!;
                        if (supplierSubAttribute is null)
                        {
                            warnings.Add($"Could not find SupplierSubscriptionAttribute with ID {attribute.SupplierSubscriptionAttributeID}");
                        }
                        else
                        {
                            supplierSubAttribute.Value = attribute.Value;
                            success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        warnings.Add($"Could not update value for attribute {attribute.SupplierSubscriptionAttributeID}");
                    }
                }
                if (_context.ChangeTracker.HasChanges())
                {
                    await _context.SaveChangesAsync();
                }
            }

            return new SupplierAttributeUpdateResponse() { Warnings = warnings, Success = success };
        }
    }
}
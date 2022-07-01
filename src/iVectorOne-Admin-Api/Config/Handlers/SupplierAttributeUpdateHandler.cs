using AutoMapper;
using iVectorOne_Admin_Api.Config.Context;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class SupplierAttributeUpdateHandler : IRequestHandler<SupplierAttributeUpdateRequest, SupplierAttributeUpdateResponse>
    {
        private ConfigContext _context;
        private IMapper _mapper;

        public SupplierAttributeUpdateHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        async Task<SupplierAttributeUpdateResponse> IRequestHandler<SupplierAttributeUpdateRequest, SupplierAttributeUpdateResponse>.Handle(SupplierAttributeUpdateRequest request, CancellationToken cancellationToken)
        {
            var warnings = new List<string>();
            bool success = false;
            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId)
                                            .Include(t => t.Subscriptions.Where(s => s.SubscriptionId == request.SubscriptionId))
                                            .ThenInclude(s => s.SupplierSubscriptionAttributes
                                                            .Where(ssa =>
                                                                    ssa.SupplierSubscriptionAttributeId == request.SupplierSubscriptionAttributeId
                                                                    && ssa.SupplierAttribute.SupplierId == request.SupplierId))
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
                try
                {
                    var supplierSubAttribute = tenant.Subscriptions.FirstOrDefault()?.SupplierSubscriptionAttributes.FirstOrDefault()!;
                    supplierSubAttribute.Value = request.UpdatedValue;
                    await _context.SaveChangesAsync();
                    success = true;
                }
                catch (Exception ex)
                {
                    warnings.Add(ex.Message.ToString());
                }
            }

            return new SupplierAttributeUpdateResponse() { Warnings = warnings, Success = success };
        }
    }
}
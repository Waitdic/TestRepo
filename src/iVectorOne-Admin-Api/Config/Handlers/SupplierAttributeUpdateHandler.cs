namespace iVectorOne_Admin_Api.Config.Handlers
{
    using iVectorOne_Admin_Api.Config.Requests;
    using iVectorOne_Admin_Api.Config.Responses;

    public class SupplierAttributeUpdateHandler : IRequestHandler<SupplierAttributeUpdateRequest, SupplierAttributeUpdateResponse>
    {
        private readonly AdminContext _context;

        public SupplierAttributeUpdateHandler(AdminContext context, IMapper mapper)
        {
            _context = context;
        }

        async Task<SupplierAttributeUpdateResponse> IRequestHandler<SupplierAttributeUpdateRequest, SupplierAttributeUpdateResponse>.Handle(SupplierAttributeUpdateRequest request, CancellationToken cancellationToken)
        {
            var warnings = new List<string>();
            bool success = false;
            var tenant = _context.Tenants
                    .Where(t => t.TenantId == request.TenantId)
                    .Include(t => t.Accounts.Where(s => s.AccountId == request.AccountId))
                    .ThenInclude(s => s.AccountSupplierAttributes.Where(ssa => ssa.SupplierAttribute.SupplierId == request.SupplierId))
                    .FirstOrDefault();

            if (tenant is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoTenantWarning);
            }

            if (!warnings.Any() && tenant?.Accounts.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoAccountWarning);
            }

            if (!warnings.Any() && tenant?.Accounts.FirstOrDefault()?.AccountSupplierAttributes.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.SingleNoSupplierAttributeWarning);
            }

            if (!warnings.Any())
            {
                foreach (var attribute in request.Attributes)
                {
                    try
                    {
                        var supplierSubAttribute = tenant?.Accounts.FirstOrDefault()?.AccountSupplierAttributes.FirstOrDefault(x => x.AccountSupplierAttributeId == attribute.AccountSupplierAttributeID)!;
                        if (supplierSubAttribute is null)
                        {
                            warnings.Add($"Could not find AccountSupplierAttribute with ID {attribute.AccountSupplierAttributeID}");
                        }
                        else
                        {
                            supplierSubAttribute.Value = attribute.Value;
                            success = true;
                        }
                    }
                    catch
                    {
                        success = false;
                        warnings.Add($"Could not update value for attribute {attribute.AccountSupplierAttributeID}");
                    }
                }
                if (_context.ChangeTracker.HasChanges())
                {
                    await _context.SaveChangesAsync();
                }
            }

            return new SupplierAttributeUpdateResponse()
            {
                Warnings = warnings,
                Success = success
            };
        }
    }
}
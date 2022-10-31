namespace iVectorOne_Admin_Api.Config.Handlers
{
    using iVectorOne_Admin_Api.Config.Requests;
    using iVectorOne_Admin_Api.Config.Responses;

    public class AccountSupplierCreateHandler : IRequestHandler<AccountSupplierCreateRequest, AccountSupplierCreateResponse>
    {
        private readonly AdminContext _context;

        public AccountSupplierCreateHandler(AdminContext context, IMapper mapper)
        {
            _context = context;
        }

        async Task<AccountSupplierCreateResponse> IRequestHandler<AccountSupplierCreateRequest, AccountSupplierCreateResponse>.Handle(AccountSupplierCreateRequest request, CancellationToken cancellationToken)
        {
            var warnings = new List<string>();
            bool success = false;
            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId)
                                            .Include(t => t.Accounts.Where(s => s.AccountId == request.AccountId))
                                            .FirstOrDefault();
            var supplierattributes = _context.SupplierAttributes.Where(sa => sa.SupplierId == request.SupplierId);

            if (tenant is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoTenantWarning);
            }

            if (!warnings.Any() && tenant?.Accounts.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoAccountWarning);
            }

            if (!warnings.Any() && supplierattributes.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.SingleNoSupplierAttributeWarning);
            }

            if (!warnings.Any())
            {
                if (_context.AccountSuppliers.Any(ss => ss.SupplierId == request.SupplierId && ss.AccountId == request.AccountId))
                {
                    warnings.Add("An entry already exists for this account/supplier combination");
                }
                else
                {
                    var accountSupplier = new AccountSupplier() { SupplierId = (short)request.SupplierId, AccountId = request.AccountId, Enabled = true };
                    _context.AccountSuppliers.Add(accountSupplier);
                }

                var accountSupplierAttributes = new List<AccountSupplierAttribute>();
                foreach (var supplierAttribute in request.SupplierAttributes)
                {
                    try
                    {
                        var attribute = new AccountSupplierAttribute()
                        {
                            AccountId = request.AccountId,
                            SupplierAttributeId = supplierAttribute.SupplierAttributeId,
                            Value = supplierAttribute.Value
                        };

                        accountSupplierAttributes.Add(attribute);
                    }
                    catch
                    {
                        success = false;
                    }
                }

                _context.AccountSupplierAttributes.AddRange(accountSupplierAttributes);

                if (!warnings.Any() && _context.ChangeTracker.HasChanges())
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    success = true;
                }
            }

            return new AccountSupplierCreateResponse()
            {
                Warnings = warnings,
                Success = success
            };
        }
    }
}
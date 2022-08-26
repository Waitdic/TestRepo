namespace iVectorOne_Admin_Api.Config.Handlers
{
    using iVectorOne_Admin_Api.Config.Models;
    using iVectorOne_Admin_Api.Config.Requests;
    using iVectorOne_Admin_Api.Config.Responses;

    public class AccountSupplierUpdateHandler : IRequestHandler<AccountSupplierUpdateRequest, AccountSupplierUpdateResponse>
    {
        private readonly ConfigContext _context;

        public AccountSupplierUpdateHandler(ConfigContext context)
        {
            _context = context;
        }

        async Task<AccountSupplierUpdateResponse> IRequestHandler<AccountSupplierUpdateRequest, AccountSupplierUpdateResponse>.Handle(AccountSupplierUpdateRequest request, CancellationToken cancellationToken)
        {
            List<SupplierDTO> suppliers = new List<SupplierDTO>();
            var warnings = new List<string>();
            bool success = false;

            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId).Include(t => t.Accounts.Where(s=> s.AccountId == request.AccountId))
                                                                                        .ThenInclude(s => s.AccountSuppliers
                                                                                            .Where(ss => ss.SupplierId == request.SupplierId && ss.AccountId == request.AccountId))
                                                                                            .ThenInclude(s => s.Supplier).FirstOrDefault();

            if (tenant is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoTenantWarning);
            }

            if (!warnings.Any() && tenant?.Accounts.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoAccountWarning);
            }

            if (!warnings.Any() && tenant?.Accounts.FirstOrDefault()?.AccountSuppliers.FirstOrDefault() is null)
            {
                warnings.Add(Warnings.ConfigWarnings.NoSupplierWarning);
            }

            if (!warnings.Any())
            {
                try
                {
                    var supplierSub = tenant?.Accounts.FirstOrDefault()?.AccountSuppliers.FirstOrDefault()!;
                    supplierSub.Enabled = request.Enabled;
                    await _context.SaveChangesAsync();
                    success = true;
                }
                catch (Exception ex)
                {
                    warnings.Add(ex.Message.ToString());
                }
            }

            return new AccountSupplierUpdateResponse() { Warnings = warnings, Success = success };
        }
    }
}
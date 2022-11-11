namespace iVectorOne_Admin_Api.Config.Handlers
{
    using iVectorOne_Admin_Api.Config.Models;
    using iVectorOne_Admin_Api.Config.Requests;
    using iVectorOne_Admin_Api.Config.Responses;

    public class AccountSupplierHandler : IRequestHandler<AccountSupplierRequest, AccountSupplierResponse>
    {
        private readonly AdminContext _context;
        private readonly IMapper _mapper;

        public AccountSupplierHandler(AdminContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        Task<AccountSupplierResponse> IRequestHandler<AccountSupplierRequest, AccountSupplierResponse>.Handle(AccountSupplierRequest request, CancellationToken cancellationToken)
        {
            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId).Include(t => t.Accounts)
                                                                                        .ThenInclude(s => s.AccountSuppliers)
                                                                                            .ThenInclude(s => s.Supplier).FirstOrDefault();
            Account? account = null;
            List<SupplierDTO> suppliers = new List<SupplierDTO>();
            var warnings = new List<string>();
            bool success = false;
            if (tenant != null)
            {
                account = tenant.Accounts.Where(s => s.AccountId == request.AccountId).FirstOrDefault();
                if (account != null)
                {
                    suppliers = _mapper.Map<List<SupplierDTO>>(account.AccountSuppliers);
                    success = true;
                }
                else
                {
                    warnings.Add(Warnings.ConfigWarnings.NoAccountWarning);
                }
            }
            else
            {
                warnings.Add(Warnings.ConfigWarnings.NoTenantWarning);
            }

            return Task.FromResult(new AccountSupplierResponse()
            {
                AccountSuppliers = suppliers,
                Warnings = warnings,
                Success = success
            });
        }
    }
}
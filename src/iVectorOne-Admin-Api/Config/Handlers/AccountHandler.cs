namespace iVectorOne_Admin_Api.Config.Handlers
{
    using iVectorOne_Admin_Api.Config.Models;
    using iVectorOne_Admin_Api.Config.Requests;
    using iVectorOne_Admin_Api.Config.Responses;

    public class AccountHandler : IRequestHandler<AccountRequest, AccountResponse>
    {
        private readonly ConfigContext _context;
        private readonly IMapper _mapper;

        public AccountHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        Task<AccountResponse> IRequestHandler<AccountRequest, AccountResponse>.Handle(AccountRequest request, CancellationToken cancellationToken)
        {
            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId).Include(t => t.Accounts).FirstOrDefault();
            Account? account = null;
            AccountDTO? accountDTO = new();
            var warnings = new List<string>();
            bool success = false;

            if (tenant != null)
            {
                account = tenant.Accounts.FirstOrDefault(s => s.AccountId == request.AccountId);
                if (account != null)
                {
                    accountDTO = _mapper.Map<AccountDTO>(account);
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

            return Task.FromResult(new AccountResponse() { Account = accountDTO, Warnings = warnings, Success = success });
        }
    }
}
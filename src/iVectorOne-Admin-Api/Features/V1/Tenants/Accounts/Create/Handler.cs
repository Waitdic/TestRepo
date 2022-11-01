namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Create
{
    using Intuitive;
    using Intuitive.Helpers.Security;
    using System.Security.Cryptography;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly AdminContext _context;
        private readonly IMapper _mapper;
        private readonly ISecretKeeper _secretKeeper;

        public Handler(AdminContext context, IMapper mapper, ISecretKeeper secretKeeper)
        {
            _context = Ensure.IsNotNull(context, nameof(context));
            _mapper = Ensure.IsNotNull(mapper, nameof(mapper));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var tenant = _context.Tenants
                .Where(t => t.TenantId == request.TenantId)
                .Include(t => t.Accounts)
                .FirstOrDefault();

            if (tenant is null)
            {
                response.NotFound();
                return response;
            }

            Account CreateAccount(bool isLive)
            {
                var account = _mapper.Map<AccountDto, Account>(request.Account);
                var environment = isLive ? "Live" : "Test";

                account.TenantId = request.TenantId;
                account.DummyResponses = false;
                account.LogMainSearchError = true;
                account.Environment = environment.ToLower();
                account.Login = $"{account.Login.Replace(' ', '_')}_{environment}";
                account.Password = account.Login; //needs to be fixed in database to allow null;
                account.EncryptedPassword = _secretKeeper.Encrypt(account.Login); // default password to login
                return account;
            }

            var accounts = new[]
            {
                CreateAccount(true),
                CreateAccount(false)
            };

            if (tenant.Accounts.Any(s => accounts.Any(n => n.Login == s.Login)))
            {
                response.Warnings.Add("An entry with the same name already exists for this Tenant");

                response.BadRequest();
                return response;
            }

            tenant.Accounts.AddRange(accounts);
            await _context.SaveChangesAsync(cancellationToken);

            response.Default(new ResponseModelBase { Success = true });
            return response;
        }
    }
}

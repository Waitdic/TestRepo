namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Update
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

            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId)
                .Include(t => t.Accounts)
                .FirstOrDefault();

            if (tenant is null)
            {
                response.NotFound();
                return response;
            }

            var account = tenant.Accounts.FirstOrDefault(s => s.AccountId == request.AccountId);

            if (account is null)
            {
                response.NotFound();
                return response;
            }

            _mapper.Map(request.Account, account);

            account.EncryptedPassword = _secretKeeper.Encrypt(request.Account.Password);

            await _context.SaveChangesAsync(cancellationToken);

            response.Default();
            return response;
        }
    }
}
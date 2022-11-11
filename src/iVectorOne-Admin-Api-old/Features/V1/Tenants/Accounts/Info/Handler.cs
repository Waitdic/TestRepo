using Intuitive;
using Intuitive.Helpers.Security;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Info
{
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ConfigContext _context;
        private readonly IMapper _mapper;
        private readonly ISecretKeeper _secretKeeper;

        public Handler(ConfigContext context, IMapper mapper, ISecretKeeper secretKeeper)
        {
            _context = context;
            _mapper = mapper;
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var account  = await _context.Accounts
                .Where (x => x.AccountId == request.AccountId && x.TenantId == request.TenantId)
                .FirstOrDefaultAsync();

            if (account == null)
            {
                response.NotFound();
                return response;
            }

            var accountDto = _mapper.Map<AccountDto>(account);
            if(account.EncryptedPassword != null)
            {
                accountDto.Password = _secretKeeper.Decrypt(account.EncryptedPassword);
            }

            response.Default(accountDto);

            return response;
        }
    }
}
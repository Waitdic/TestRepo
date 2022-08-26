namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Info
{
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ConfigContext _context;
        private readonly IMapper _mapper;

        public Handler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            response.Default(accountDto);

            return response;
        }
    }
}
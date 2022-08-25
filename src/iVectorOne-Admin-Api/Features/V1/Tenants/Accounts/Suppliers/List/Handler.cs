namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.List
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

        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var account = _context.Accounts
                .Where(x => x.TenantId == request.TenantId && x.AccountId == request.AccountId)
                .Include(x => x.AccountSuppliers)
                .ThenInclude(x => x.Supplier)
                .AsNoTracking()
                .FirstOrDefault();

            if (account == null)
            {
                response.NotFound();
                return Task.FromResult(response);
            }

            var suppliers = _mapper.Map<List<SupplierDto>>(account.AccountSuppliers);

            response.Default(new AccountDto { AccountId = request.AccountId, AccountSuppliers = suppliers});

            return Task.FromResult(response);
        }
    }
}
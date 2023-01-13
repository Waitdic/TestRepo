using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.List
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;
        private readonly IMapper _mapper;

        public Handler(AdminContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var account = await _context.Accounts
                .Where(x => x.TenantId == request.TenantId && x.AccountId == request.AccountId)
                .Include(x => x.AccountSuppliers)
                .ThenInclude(x => x.Supplier)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (account == null)
            {
                response.NotFound();
                return response;
            }

            var suppliers = _mapper.Map<List<SupplierDto>>(account.AccountSuppliers);

            response.Ok(new ResponseModel { AccountId = request.AccountId, AccountSuppliers = suppliers});

            return response;
        }
    }
}
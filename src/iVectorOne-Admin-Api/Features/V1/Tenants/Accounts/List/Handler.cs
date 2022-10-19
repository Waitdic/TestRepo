namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.List
{
    using iVectorOne_Admin_Api.Config.Models;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly AdminContext _context;
        private readonly IMapper _mapper;

        public Handler(AdminContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var tenant = await _context.Tenants
                            .Where(t => t.TenantId == request.TenantId)
                            .Include(t => t.Accounts.Where(s => s.Status == RecordStatus.Active)).FirstOrDefaultAsync();

            if (tenant == null)
            {
                response.NotFound();
                return response;
            }

            var accounts = _mapper.Map<List<AccountDto>>(tenant.Accounts);

            response.Default(new TenantDto { TenantId = tenant.TenantId, Accounts = accounts, TenantName = tenant.CompanyName });

            return response;
        }
    }
}
using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.List
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

            var tenantModels = await _context.Tenants.Where(t => t.Status != RecordStatus.Deleted)
                .OrderBy(t => t.CompanyName)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var tenants = _mapper.Map<List<TenantDto>>(tenantModels);

            response.Default(new ResponseModel { Success = true, Tenants = tenants });

            return response;
        }
    }
}

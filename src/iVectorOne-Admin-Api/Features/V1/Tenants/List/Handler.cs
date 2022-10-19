using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.List
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

            var tenants = await _mapper.ProjectTo<TenantDto>(_context.Tenants)
                .Where(t => t.Status != RecordStatus.Deleted)
                .OrderBy(t => t.CompanyName)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            response.Ok(new ResponseModel { Success = true, Tenants = tenants });

            return response;
        }
    }
}

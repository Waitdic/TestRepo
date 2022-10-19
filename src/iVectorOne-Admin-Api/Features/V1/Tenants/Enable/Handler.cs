using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Enable
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

            var tenant = await _context.Tenants.Where(t => t.TenantId == request.TenantId)
                .FirstOrDefaultAsync();

            if (tenant == null)
            {
                response.NotFound("Tenant not found.");
                return response;
            }

            tenant.Status = RecordStatus.Active;
            await _context.SaveChangesAsync(cancellationToken);

            response.Ok(new ResponseModel { Success = true, TenantId = request.TenantId });
            return response;
        }
    }
}

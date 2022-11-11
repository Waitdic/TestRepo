

using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Disable
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

            var tenantModel = await _context.Tenants.Where(t => t.TenantId == request.TenantId)
                .FirstOrDefaultAsync();

            if (tenantModel == null)
            {
                response.NotFound();
                return response;
            }

            tenantModel.Status = RecordStatus.Inactive;
            await _context.SaveChangesAsync();

            response.Default(new ResponseModel { Success = true, TenantId = request.TenantId });

            return response;
        }
    }
}

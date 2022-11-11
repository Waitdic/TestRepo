using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Delete
{
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ConfigContext _context;

        public Handler(ConfigContext context, IMapper mapper)
        {
            _context = context;
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

            tenantModel.Status = RecordStatus.Deleted;
            await _context.SaveChangesAsync();

            response.Default(new ResponseModel { Success = true, TenantId = request.TenantId});

            return response;
        }
    }
}

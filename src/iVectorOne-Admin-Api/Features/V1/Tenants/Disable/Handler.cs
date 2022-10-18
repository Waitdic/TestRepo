

using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Disable
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly ConfigContext _context;

        public Handler(ConfigContext context)
        {
            _context = context;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var tenant = await _context.Tenants.Where(t => t.TenantId == request.TenantId)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (tenant == null)
            {
                response.NotFound("Tenant not found.");
                return response;
            }

            tenant.Status = RecordStatus.Inactive;
            await _context.SaveChangesAsync(cancellationToken);

            response.Ok(new ResponseModel { Success = true, TenantId = request.TenantId });

            return response;
        }
    }
}

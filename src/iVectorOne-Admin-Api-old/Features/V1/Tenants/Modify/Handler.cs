

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Modify
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

            tenantModel.ContactEmail = request.ContactEmail;
            tenantModel.ContactName = request.ContactName;
            tenantModel.ContactTelephone = request.ContactTelephone;

            await _context.SaveChangesAsync();

            response.Default(new ResponseModel { Success = true });

            return response;
        }
    }
}

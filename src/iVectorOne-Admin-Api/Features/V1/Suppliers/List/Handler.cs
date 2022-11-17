using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Suppliers.List
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;

        public Handler(AdminContext context)
        {
            _context = context;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var suppliers = await _context.Suppliers.AsNoTracking().ToListAsync();

            var supplierList = suppliers.Select(x => new SupplierDto
            {
                SupplierID = x.SupplierId,
                Name = x.SupplierName
            }).OrderBy(x => x.Name).ToList();

            response.Ok(new ResponseModel { Success = true, Suppliers = supplierList });

            return response;
        }
    }
}
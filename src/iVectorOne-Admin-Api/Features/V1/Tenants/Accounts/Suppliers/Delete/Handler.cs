namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Delete
{
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly AdminContext _context;

        public Handler(AdminContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId)
                .Include(t => t.Accounts)
                .ThenInclude(t => t.AccountSuppliers)
                .FirstOrDefault();

            if (tenant is null)
            {
                response.NotFound();
                return response;
            }

            var account = tenant.Accounts.FirstOrDefault(s => s.AccountId == request.AccountId);

            if (account is null)
            {
                response.NotFound();
                return response;
            }

            var accountSupplier = account.AccountSuppliers.FirstOrDefault(s => s.SupplierId == request.SupplierId);

            if (accountSupplier is null)
            {
                response.NotFound();
                return response;
            }

            _context.AccountSuppliers.Remove(accountSupplier);
            await _context.SaveChangesAsync(cancellationToken);

            var accountSupplierAttributes = _context.AccountSupplierAttributes.Where(s => s.AccountId == request.AccountId).Include(x => x.SupplierAttribute).ToList();
            var accountSupplierAttributesDelete = accountSupplierAttributes.Where(x => x.SupplierAttribute.SupplierId == request.SupplierId);
            _context.AccountSupplierAttributes.RemoveRange(accountSupplierAttributesDelete);
            await _context.SaveChangesAsync(cancellationToken);

            response.Default();

            return response;
        }
    }
}
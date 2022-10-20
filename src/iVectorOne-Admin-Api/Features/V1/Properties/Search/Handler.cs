using FluentValidation;
using FluentValidation.Results;
using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Properties.Search
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;
        private readonly IValidator<Request> _validator;

        public Handler(AdminContext context, IValidator<Request> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var account = _context.Accounts.Where(a => a.AccountId == request.AccountID)
                .Include(a => a.AccountSuppliers)
                .ThenInclude(a => a.Supplier)
                .AsNoTracking()
                .FirstOrDefault();

            if (account == null)
            {
                response.NotFound("Account not found.");
                return response;
            }

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                response.BadRequest("One or more parameters contained invalid values.", validationResult.ToDictionary());
                return response;
            }

            //SELECT DISTINCT TOP 20 T1.Name, T2.CentralPropertyID
            //FROM Property T1
            //INNER JOIN PropertyDedupe T2 ON T2.PropertyID = T1.PropertyID
            //INNER JOIN Supplier T3 ON T3.SupplierName = T2.Source
            //INNER JOIN AccountSupplier T4 ON T4.SupplierID = T3.SupplierID
            //WHERE Name LIKE 'Centara %'
            //AND T4.AccountID = 2

            response.Ok(new ResponseModel() { Success = true });
            return response;
        }
    }
}

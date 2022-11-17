using FluentValidation;
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

            //Check the tenant and account that was supplied are valid
            var account = await AccountChecker.IsTenantAccountValid(_context, request.TenantId, request.AccountId, cancellationToken);

            if (account == null)
            {
                response.NotFound();
                return response;
            }

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                response.BadRequest("One or more parameters contained invalid values.", validationResult.ToDictionary());
                return response;
            }

            //Escape any single quote in the query so the query doesn't break
            var query = request.Query.Replace("'", "''");

            //Append a % for wildcard match if not supplied
            var queryText = $"Portal_PropertySearch '{(query!.EndsWith('%') ? query : $"{query}%")}', {request.AccountID}";
            var properties = await _context.Properties
                .FromSqlRaw(queryText)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var propertyList = properties.Select(x => new PropertyDto
            {
                PropertyId = x.CentralPropertyID,
                Name = x.Name,
            }).ToList();

            response.Ok(new ResponseModel() { Success = true, Properties = propertyList });
            return response;
        }
    }
}

using AutoMapper.QueryableExtensions;
using FluentValidation;
using iVectorOne_Admin_Api.Data;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Properties.Search
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;
        private readonly IValidator<Request> _validator;
        private readonly IMapper _mapper;

        public Handler(AdminContext context, IValidator<Request> validator, IMapper mapper)
        {
            _context = context;
            _validator = validator;
            _mapper = mapper;
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

            var query = request.Query.Replace("'", "''");
            var queryText = $"Portal_PropertySearch '{(query!.EndsWith('%') ? query : $"{query}%")}', {request.AccountID}";
            var properties = await _context.Properties.FromSqlRaw(queryText).ToListAsync(cancellationToken: cancellationToken);

            var propertyList = _mapper.Map<List<PropertyDto>>(properties);

            response.Ok(new ResponseModel() { Success = true, Properties = propertyList });
            return response;
        }
    }
}

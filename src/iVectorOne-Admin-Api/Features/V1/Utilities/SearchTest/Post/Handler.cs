using FluentValidation;
using FluentValidation.Results;
using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Adaptors.Search.FireForget;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest.Post
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;

        private readonly IAdaptor<Adaptors.Search.Request, Adaptors.Search.Response> _searchAdaptor;
        private readonly IValidator<Request> _validator;
        private readonly IFireForgetSearchHandler _fireForgetSearchHandler;

        public Handler(AdminContext context,
            IAdaptor<Adaptors.Search.Request, Adaptors.Search.Response> searchAdaptor,
            IValidator<Request> validator,
            IFireForgetSearchHandler fireForgetSearchHandler)
        {
            _context = context;
            _searchAdaptor = searchAdaptor;
            _validator = validator;
            _fireForgetSearchHandler = fireForgetSearchHandler;
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

            if (string.IsNullOrEmpty(account.EncryptedPassword))
            {
                response.Ok(new ResponseModel { Success = true, Message = "Sorry, there are no tests configured for this supplier." });
                return response;
            }

            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                response.BadRequest("", validationResult.ToDictionary());
                return response;
            }

            var searchRequest = new Adaptors.Search.Request
            {
                RequestKey = Guid.NewGuid(),
                Searchdate = request.SearchRequest.ArrivalDate,
                Properties = string.Join(", ", request.SearchRequest.Properties),
                Login = account.Login,
                Password = account.EncryptedPassword,
                DedupeMethod = "none",
                RoomRequest = request.SearchRequest.RoomRequestsToString()
            };

            _fireForgetSearchHandler.Execute(async repository =>
            {
                await _searchAdaptor.Execute(searchRequest);
            });

            response.Accepted(new ResponseModel { Success = true, Message = "", RequestKey = searchRequest.RequestKey });
            return response;
        }
    }
}

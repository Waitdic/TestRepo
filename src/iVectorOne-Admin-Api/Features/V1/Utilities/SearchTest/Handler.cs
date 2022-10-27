using FluentValidation;
using FluentValidation.Results;
using Intuitive;
using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Adaptors.Search.FireForget;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;

        private readonly IAdaptor<Adaptors.Search.Request, Adaptors.Search.Response> _searchAdaptor;
        private readonly IValidator<Request> _validator;
        private readonly IFireForgetSearchHandler _fireForgetSearchHandler;
        private readonly IFireForgetSearchOperation _fireForgetOperation;

        public Handler(AdminContext context,
            IFireForgetSearchOperation fireForgetOperation,
            IAdaptor<Adaptors.Search.Request, Adaptors.Search.Response> searchAdaptor,
            IValidator<Request> validator,
            IFireForgetSearchHandler fireForgetSearchHandler)
        {
            _context = context;
            _fireForgetOperation = fireForgetOperation;
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
                // Will receive its own scoped repository on the executing task
                //await repository.AuditBlogUpdate(new BlogAudit(blog));
                //await _searchAdaptor.Execute(searchRequest, cancellationToken);
                await _searchAdaptor.Execute(searchRequest);
            });

            //var result = await _searchAdaptor.Execute(searchRequest, cancellationToken);

            //var searchResults = new List<SearchResult>();

            //if (result.SearchStatus == Adaptors.Search.Response.SearchStatusEnum.Ok)
            //{
            //    foreach (var property in result.SearchResult.PropertyResults)
            //    {
            //        foreach (var roomType in property.RoomTypes)
            //        {
            //            searchResults.Add(new SearchResult
            //            {
            //                Supplier = roomType.Supplier,
            //                RoomCode = roomType.RateCode,
            //                RoomType = roomType.SupplierRoomType,
            //                MealBasis = roomType.MealBasisCode,
            //                Currency = roomType.CurrencyCode,
            //                TotalCost = roomType.TotalCost,
            //                NonRefundable = roomType.NonRefundable
            //            });
            //        }
            //    }
            //}

            response.Ok(new ResponseModel { Success = true, Message = "", RequestKey = searchRequest.RequestKey });
            return response;
        }
    }
}

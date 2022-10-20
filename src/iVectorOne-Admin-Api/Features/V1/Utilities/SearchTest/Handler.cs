﻿using FluentValidation;
using FluentValidation.Results;
using Intuitive;
using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;
        private readonly IAdaptor<Adaptors.Search.Request, Adaptors.Search.Response> _searchAdaptor;
        private readonly IValidator<Request> _validator;

        public Handler(AdminContext context,
            IAdaptor<Adaptors.Search.Request, Adaptors.Search.Response> searchAdaptor,
            IValidator<Request> validator)
        {
            _context = context;
            _searchAdaptor = searchAdaptor;
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
                Searchdate = request.SearchRequest.ArrivalDate,
                Properties = string.Join(", ", request.SearchRequest.Properties),
                Login = account.Login,
                Password = account.EncryptedPassword,
                DedupeMethod = "none",
                RoomRequest = "(2,0,0)"
            };

            var result = await _searchAdaptor.Execute(searchRequest, cancellationToken);

            var searchResults = new List<SearchResult>();

            if (result.SearchStatus == Adaptors.Search.Response.SearchStatusEnum.Ok)
            {
                foreach (var property in result.SearchResult.PropertyResults)
                {
                    foreach (var roomType in property.RoomTypes)
                    {
                        searchResults.Add(new SearchResult
                        {
                            Supplier = roomType.Supplier,
                            RoomCode = roomType.RateCode,
                            RoomType = roomType.SupplierRoomType,
                            MealBasis = roomType.MealBasisCode,
                            Currency = roomType.CurrencyCode,
                            TotalCost = roomType.TotalCost,
                            NonRefundable = roomType.NonRefundable
                        });
                    }
                }
            }

            response.Ok(new ResponseModel { Success = true, Results = searchResults, Message = result.Information });
            return response;
        }
    }
}

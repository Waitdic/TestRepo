using FluentValidation;
using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Data;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest.Get
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

            var results = await _context.FireForgetSearchResponses
                .Where(x => x.FireForgetSearchResponseKey == request.RequestKey)
                .ToListAsync(cancellationToken: cancellationToken);

            if (results.Count == 0)
            {
                response.NotReady();
                return response;
            }

            var searchResults = new List<SearchResult>();

            foreach (var result in results)
            {
                if (result.SearchStatus.ToLower() == "ok")
                {
                    foreach (var property in result.SearchResponse.PropertyResults)
                    {
                        foreach (var roomType in property.RoomTypes)
                        {
                            searchResults.Add(new SearchResult
                            {
                                Supplier = roomType.Supplier,
                                RoomCode = roomType.Code,
                                RoomType = roomType.SupplierRoomType,
                                MealBasis = roomType.MealBasisCode,
                                Currency = roomType.CurrencyCode,
                                TotalCost = roomType.TotalCost,
                                NonRefundable = roomType.NonRefundable
                            });
                        }
                    }
                }
            }

            var message = results.Select(x => x.Information).Aggregate((i, j) => i + ',' + j);

            //Delete the response once it's been processed
            foreach (var result in results)
            {
                _context.FireForgetSearchResponses.Remove(result);
                await _context.SaveChangesAsync(cancellationToken);
            }

            response.Ok(new ResponseModel { Success = true, Message=message, Results = searchResults });

            return response;
        }
    }
}

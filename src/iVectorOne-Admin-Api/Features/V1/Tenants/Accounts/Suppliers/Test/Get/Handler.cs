using FluentValidation;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test.Get
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

            if (results.Count < 3)
            {
                response.NotReady();
                return response;
            }

            //Delete the response once it's been processed
            foreach (var result in results)
            {
                _context.FireForgetSearchResponses.Remove(result);
                await _context.SaveChangesAsync(cancellationToken);
            }

            if (results.Any(x => x.SearchStatus.ToLower() == "ok"))
            {
                response.Ok(new ResponseModel { Success = true, Message = "Success. The supplier is returning results." });
                return response;
            }
            else
            {
                response.Ok(new ResponseModel { Success = true, Message = $"Failed with error: {string.Join(",", results.Where(x => x.Information.Length != 0).ToList())}" });
                return response;
            }

            //    var cancelSearch = false;
            //    var searchRequest = new Adaptors.Search.Request
            //    {
            //        Searchdate = searchDate.AddMonths(i * 3),
            //        Properties = supplier.TestPropertyIDs,
            //        Login = account.Login,
            //        Password = account.EncryptedPassword,
            //        DedupeMethod = "none",
            //        RoomRequest = "(2,0,0)"
            //    };

            //    var result = await _searchAdaptor.Execute(searchRequest, cancellationToken);
            //    //var result = await ExecuteSearch(supplier.TestPropertyIDs, searchDate.AddMonths(i * 3), account.Login, account.EncryptedPassword);
            //    switch (result.SearchStatus)
            //    {
            //        case Adaptors.Search.Response.SearchStatusEnum.Ok:
            //            message = "Success. The supplier is returning results.";
            //            break;
            //        case Adaptors.Search.Response.SearchStatusEnum.Exception:
            //            message = $"Failed with unexpected error: {result.Information}";
            //            cancelSearch = true;
            //            break;
            //        case Adaptors.Search.Response.SearchStatusEnum.NotOk:
            //            message = $"Failed with error: {result.Information}";
            //            cancelSearch = true;
            //            break;
            //    };

            //    if (cancelSearch)
            //    {
            //        break;
            //    }

            response.Ok(new ResponseModel { Success = true, Message = "" });
            return response;
        }
    }
}

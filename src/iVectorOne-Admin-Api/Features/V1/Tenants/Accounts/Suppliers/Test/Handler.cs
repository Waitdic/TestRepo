using Intuitive;
using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;
        private readonly IAdaptor<Adaptors.Search.Request, Adaptors.Search.Response> _searchAdaptor;

        public Handler(AdminContext context, IAdaptor<Adaptors.Search.Request, Adaptors.Search.Response> searchAdaptor)
        {
            _context = context;
            _searchAdaptor = searchAdaptor;
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

            var supplier = account!.AccountSuppliers.FirstOrDefault(a => a.SupplierId == request.SupplierID)!.Supplier;

            if (supplier == null)
            {
                response.NotFound("Supplier not found.");
                return response;
            }

            if (string.IsNullOrEmpty(supplier.TestPropertyIDs) || string.IsNullOrEmpty(account.EncryptedPassword))
            {
                response.Ok(new ResponseModel { Success = true, Message = "Sorry, there are no tests configured for this supplier." });
                return response;
            }


            var message = "Sorry, no results could be found.";
            var searchDate = DateTime.Today.AddDays(1);

            //for (int i = 0; i < 3; i++)
            //{
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
            //}


            response.Ok(new ResponseModel { Success = true, Message = message });
            return response;
        }
    }
}

using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Adaptors.Search;
using iVectorOne_Admin_Api.Adaptors.Search.FireForget;
using iVectorOne_Admin_Api.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest
{
    public class SearchOperation : IFireForgetSearchOperation
    {
        private readonly IAdaptor<Adaptors.Search.Request, Adaptors.Search.Response> _searchAdaptor;
        private readonly AdminContext _context;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SearchOperation(IAdaptor<Adaptors.Search.Request, Adaptors.Search.Response> searchAdaptor, AdminContext context, IServiceScopeFactory serviceScopeFactory)
        {
            _searchAdaptor = searchAdaptor;
            _context = context;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(Adaptors.Search.Request request)
        {
            var result = await _searchAdaptor.Execute(request);

            var propertySearchResults = new FireForgetSearchResponse
            {
                //Message = result.Information,
                SearchResponse = result.SearchResult,
                //SearchKey = "asda"

            };

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AdminContext>();
                context.FireForgetSearchResponses.Add(propertySearchResults);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

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
        }
    }
}

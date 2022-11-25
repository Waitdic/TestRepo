namespace iVectorOne.Suppliers.Polaris
{
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PolarisSearch : IThirdPartySearch, ISingleSource
    {
        public string Source => ThirdParties.POLARIS;

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var hotelBatches = resortSplits.SelectMany(resort => resort.Hotels.Select(hotel => hotel.TPKey)).Batch(250);

            var requests = hotelBatches.Select(batch => 
            {

                return new Request
                {

                };
            }).ToList();
            return Task.FromResult(requests);
        }

        public string BuildJsonRequest(SearchDetails searchDetails, List<string> tpKeys) 
        {

            return "";
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            throw new System.NotImplementedException();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Intuitive.Helpers.Net;
using iVectorOne.Interfaces;
using iVectorOne.Models;
using iVectorOne.Search.Models;
using iVectorOne.Search.Results.Models;

namespace iVectorOne.Suppliers.PremierInn
{
    public class PremierInnSearch : IThirdPartySearch, ISingleSource
    {
        public string Source { get; }

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            throw new System.NotImplementedException();
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            throw new System.NotImplementedException();
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            throw new System.NotImplementedException();
        }

        public bool ResponseHasExceptions(Request request)
        {
            throw new System.NotImplementedException();
        }
    }
}

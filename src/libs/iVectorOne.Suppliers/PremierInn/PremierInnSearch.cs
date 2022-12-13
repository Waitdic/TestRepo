using System.Collections.Generic;
using System.Threading.Tasks;
using Intuitive;
using Intuitive.Helpers.Net;
using Intuitive.Helpers.Serialization;
using iVectorOne.Constants;
using iVectorOne.Interfaces;
using iVectorOne.Models;
using iVectorOne.Search.Models;
using iVectorOne.Search.Results.Models;

namespace iVectorOne.Suppliers.PremierInn
{
    public class PremierInnSearch : IThirdPartySearch, ISingleSource
    {
        private readonly IPremierInnSettings _settings;
        private readonly ISerializer _serializer;

        public PremierInnSearch(IPremierInnSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public string Source => ThirdParties.PREMIERINN;

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

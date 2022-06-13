namespace ThirdParty.CSSuppliers.DerbySoft
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using Intuitive.Net.WebRequests;
    using ThirdParty;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    public class DerbySoftSearch : IThirdPartySearch, IMultiSource
    {
        #region "Constructor"

        private readonly IDerbySoftSettings _settings;

        public DerbySoftSearch(IDerbySoftSettings settings)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
        }

        #endregion

        #region Properties

        public List<string> Sources => DerbySoftSupport.DerbysoftSources;

        public bool ResponseHasExceptions(Request request) => false;

        public bool SearchRestrictions(SearchDetails searchDetails, string source) => false;

        #endregion

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            string source = resortSplits.First().ThirdPartySupplier;
            var requestBuilderFactory = new SearchFactory(_settings, source, Guid.NewGuid());
            var requestBuilder = requestBuilderFactory.GetSearchRequestBuilder(searchDetails);

            return requestBuilder.BuildSearchRequests(searchDetails, resortSplits).ToList();
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            string source = resortSplits.First().ThirdPartySupplier;
            var transformedResults = new TransformedResultCollection();

            var responseTransformerFactory = new SearchFactory(_settings, source, Guid.NewGuid());
            var responseTransformer = responseTransformerFactory.GetSearchResponseTransformer(searchDetails);

            transformedResults.TransformedResults.AddRange(responseTransformer.TransformResponses(requests));

            return transformedResults;
        }
    }
}
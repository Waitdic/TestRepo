namespace ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using Intuitive.Net.WebRequests;
    using ThirdParty;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    public abstract class DerbySoftBookingUsbV4Search : IThirdPartySearch
    {
        #region "Constructor"

        private readonly IDerbySoftSettings _settings;

        public DerbySoftBookingUsbV4Search(IDerbySoftSettings settings)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
        }

        #endregion

        #region Properties

        public abstract string Source { get; }

        public bool ResponseHasExceptions(Request request) => false;

        public bool SearchRestrictions(SearchDetails searchDetails) => false;

        #endregion

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requestBuilderFactory = new SearchFactory(_settings, Source, Guid.NewGuid());
            var requestBuilder = requestBuilderFactory.GetSearchRequestBuilder(searchDetails);
            var guid = Guid.NewGuid();

            return requestBuilder.BuildSearchRequests(searchDetails, resortSplits).ToList();
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var responseTransformerFactory = new SearchFactory(_settings, Source, Guid.NewGuid());
            var responseTransformer = responseTransformerFactory.GetSearchResponseTransformer(searchDetails);

            transformedResults.TransformedResults.AddRange(responseTransformer.TransformResponses(requests));

            return transformedResults;
        }
    }
}
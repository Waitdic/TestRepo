namespace ThirdParty.CSSuppliers
{
    using global::ThirdParty;
    using global::ThirdParty.Lookups;
    using global::ThirdParty.Models;
    using global::ThirdParty.Results;
    using global::ThirdParty.Search.Models;
    using Intuitive;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ThirdPartyInterfaces.DerbySoft;

    public abstract class DerbySoftBookingUsbV4Search : ThirdPartyPropertySearchBase
    {
        #region "Constructor"

        private readonly IDerbySoftSettings _settings;

        public DerbySoftBookingUsbV4Search(IDerbySoftSettings settings, ILogger logger)
            : base(logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
        }

        #endregion

        #region Properties

        public override bool SqlRequest => false;

        public override bool ResponseHasExceptions(Request request) => false;

        public override bool SearchRestrictions(SearchDetails searchDetails) => false;

        #endregion
        
        public override List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits, bool saveLogs)
        {
            var requestBuilderFactory = new SearchFactory(_settings, Source, Guid.NewGuid());
            var requestBuilder = requestBuilderFactory.GetSearchRequestBuilder(searchDetails);
            var guid = Guid.NewGuid();

            return requestBuilder.BuildSearchRequests(searchDetails, resortSplits, saveLogs).ToList();
        }

        public override TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var responseTransformerFactory = new SearchFactory(_settings, Source, Guid.NewGuid());
            var responseTransformer = responseTransformerFactory.GetSearchResponseTransformer(searchDetails);

            transformedResults.TransformedResults.AddRange(responseTransformer.TransformResponses(requests));

            return transformedResults;
        }
    }
}
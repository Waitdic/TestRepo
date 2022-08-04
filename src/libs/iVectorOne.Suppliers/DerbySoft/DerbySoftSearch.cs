﻿namespace iVectorOne.Suppliers.DerbySoft
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Net;
    using iVectorOne;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public class DerbySoftSearch : IThirdPartySearch, IMultiSource
    {
        #region Constructor

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

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            string source = resortSplits.First().ThirdPartySupplier;
            var requestBuilderFactory = new SearchFactory(_settings, source, Guid.NewGuid());
            var requestBuilder = requestBuilderFactory.GetSearchRequestBuilder(searchDetails);

            return Task.FromResult(requestBuilder.BuildSearchRequests(searchDetails, resortSplits).ToList());
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            string source = resortSplits.First().ThirdPartySupplier;
            var transformedResults = new TransformedResultCollection();

            var responseTransformerFactory = new SearchFactory(_settings, source, Guid.NewGuid());
            var responseTransformer = responseTransformerFactory.GetSearchResponseTransformer(searchDetails);

            transformedResults.TransformedResults.AddRange(responseTransformer.TransformResponses(requests, searchDetails));

            return transformedResults;
        }
    }
}
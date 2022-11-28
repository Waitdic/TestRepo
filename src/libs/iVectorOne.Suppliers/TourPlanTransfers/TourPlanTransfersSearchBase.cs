﻿namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using Intuitive;
    using Intuitive.Helpers.Net;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Transfer;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public abstract class TourPlanTransfersSearchBase : IThirdPartySearch, ISingleSource
    {
        public abstract string Source { get; }

        private ITourPlanTransfersSettings _settings;

        private readonly HttpClient _httpClient;

        public TourPlanTransfersSearchBase(
            ITourPlanTransfersSettings settings,
            HttpClient httpClient)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
        }

        public Task<List<Request>> BuildSearchRequestsAsync(TransferSearchDetails searchDetails, LocationMapping location)
        {
            throw new System.NotImplementedException();
        }

        public object GetThirdPartyLocations(TransferSearchDetails searchDetails, LocationMapping location)
        {
            throw new System.NotImplementedException();
        }

        public bool ResponseHasExceptions(Request request)
        {
            throw new System.NotImplementedException();
        }

        public bool SearchRestrictions(TransferSearchDetails searchDetails)
        {
            throw new System.NotImplementedException();
        }

        public TransformedTransferResultCollection TransformResponse(List<Request> requests, TransferSearchDetails searchDetails, LocationMapping location)
        {
            throw new System.NotImplementedException();
        }
    }
}

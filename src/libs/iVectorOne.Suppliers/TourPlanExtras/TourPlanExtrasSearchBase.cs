namespace iVectorOne.Suppliers.TourPlanExtras
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Net;
    using iVectorOne.Extra;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models.Extra;

    public abstract class TourPlanExtrasSearchBase : IThirdPartySearch, ISingleSource
    {
        public abstract string Source { get; }

        public Task<List<Request>> BuildSearchRequestsAsync(ExtraSearchDetails searchDetails, LocationMapping location)
        {
            throw new NotImplementedException();
        }

        public bool ResponseHasExceptions(Request request)
        {
            throw new NotImplementedException();
        }

        public bool SearchRestrictions(ExtraSearchDetails searchDetails)
        {
            throw new NotImplementedException();
        }

        public TransformedExtraResultCollection TransformResponse(List<Request> requests, ExtraSearchDetails searchDetails, LocationMapping location)
        {
            throw new NotImplementedException();
        }

        public bool ValidateSettings(ExtraSearchDetails searchDetails)
        {
            throw new NotImplementedException();
        }
    }
}

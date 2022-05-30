namespace ThirdPartyInterfaces.DerbySoft
{
    using System.Collections.Generic;
    using global::ThirdParty.Models;
    using global::ThirdParty.Search.Models;
    using Intuitive.Net.WebRequests;

    public interface ISearchRequestBuilder
    {
        IEnumerable<Request> BuildSearchRequests(
            SearchDetails searchDetails, 
            List<ResortSplit> resortSplits,
            bool saveLogs);
    }
}
namespace ThirdParty.CSSuppliers.DerbySoft
{
    using System.Collections.Generic;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;

    public interface ISearchRequestBuilder
    {
        IEnumerable<Request> BuildSearchRequests(
            SearchDetails searchDetails, 
            List<ResortSplit> resortSplits,
            bool saveLogs);
    }
}
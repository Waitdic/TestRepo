namespace iVectorOne.Suppliers.DerbySoft
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Net;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;

    public interface ISearchRequestBuilder
    {
        IEnumerable<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits);
    }
}
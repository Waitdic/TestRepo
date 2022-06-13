namespace ThirdParty.CSSuppliers.DerbySoft
{
    using System.Collections.Generic;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    public interface ISearchResponseTransformer
    {
        IEnumerable<TransformedResult> TransformResponses(List<Request> requests, SearchDetails searchDetails);
    }
}
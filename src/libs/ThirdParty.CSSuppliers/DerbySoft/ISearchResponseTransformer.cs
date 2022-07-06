namespace ThirdParty.CSSuppliers.DerbySoft
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Net;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;

    public interface ISearchResponseTransformer
    {
        IEnumerable<TransformedResult> TransformResponses(List<Request> requests, SearchDetails searchDetails);
    }
}
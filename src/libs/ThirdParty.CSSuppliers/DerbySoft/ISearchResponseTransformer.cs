namespace ThirdParty.CSSuppliers.DerbySoft
{
    using System.Collections.Generic;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Results;

    public interface ISearchResponseTransformer
    {
        IEnumerable<TransformedResult> TransformResponses(List<Request> requests);
    }
}
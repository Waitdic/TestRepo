namespace ThirdPartyInterfaces.DerbySoft
{
    using System.Collections.Generic;
    using global::ThirdParty.Results;
    using Intuitive.Net.WebRequests;

    public interface ISearchResponseTransformer
    {
        IEnumerable<TransformedResult> TransformResponses(List<Request> requests);
    }
}

namespace iVectorOne.CSSuppliers.DerbySoft
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Net;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public interface ISearchResponseTransformer
    {
        IEnumerable<TransformedResult> TransformResponses(List<Request> requests, SearchDetails searchDetails);
    }
}
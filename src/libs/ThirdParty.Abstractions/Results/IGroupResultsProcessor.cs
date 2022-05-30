﻿namespace ThirdParty.Results
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using iVector.Search.Property;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    /// <summary>Defines a class responsible for grouping results.</summary>
    public interface IGroupResultsProcessor
    {
        /// <summary>Groups the property results.</summary>
        /// <param name="thirdPartyResults">The third party results.</param>
        /// <param name="source">The source.</param>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="resortSplits">The resort splits.</param>
        /// <returns>
        ///   a List of grouped results
        /// </returns>
        Task<List<PropertySearchResult>> GroupPropertyResultsAsync(
            TransformedResultCollection thirdPartyResults,
            string source,
            SearchDetails searchDetails,
            IEnumerable<IResortSplit> resortSplits);
    }
}
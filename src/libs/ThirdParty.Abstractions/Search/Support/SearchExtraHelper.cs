namespace ThirdParty.Search.Support
{
    using System.Collections.Generic;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;

    /// <summary>A class used to support the search by getting unique request ids</summary>
    public class SearchExtraHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchExtraHelper"/> class.
        /// </summary>
        public SearchExtraHelper()
        {
            SearchDetails = new SearchDetails();
            // forced to have this for inheritence
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchExtraHelper"/> class.
        /// </summary>
        /// <param name="searchDetails">The o search details.</param>
        /// <param name="uniqueRequestID">The unique request identifier.</param>
        public SearchExtraHelper(SearchDetails searchDetails, string uniqueRequestID) : this(searchDetails, uniqueRequestID, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchExtraHelper"/> class.
        /// </summary>
        /// <param name="searchDetails">The o search details.</param>
        /// <param name="packageSearch">if set to <c>true</c> [package search].</param>
        public SearchExtraHelper(SearchDetails searchDetails, bool packageSearch) : this(searchDetails, string.Empty, packageSearch)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchExtraHelper"/> class.
        /// </summary>
        /// <param name="searchDetails">The o search details.</param>
        /// <param name="uniqueRequestID">The unique request identifier.</param>
        /// <param name="packageSearch">if set to <c>true</c> [package search].</param>
        public SearchExtraHelper(SearchDetails searchDetails, string uniqueRequestID, bool packageSearch)
        {
            SearchDetails = searchDetails;
            this.UniqueRequestID = uniqueRequestID;
            this.PackageSearch = packageSearch;
        }

        /// <summary>Gets or sets the search details.</summary>
        /// <value>The search details.</value>
        public SearchDetails SearchDetails { get; set; }

        /// <summary>Gets or sets the unique request identifier.</summary>
        /// <value>The unique request identifier.</value>
        public string UniqueRequestID { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether [package search].</summary>
        /// <value>
        /// <c>true</c> if [package search]; otherwise, <c>false</c>.</value>
        public bool PackageSearch { get; set; } = false;

        /// <summary>Gets or sets the extra information.</summary>
        /// <value>The extra information.</value>
        public string ExtraInfo { get; set; } = string.Empty;

        /// <summary>Gets or sets the resort splits.</summary>
        /// <value>The resort splits.</value>
        public List<ResortSplit> ResortSplits { get; set; } = new List<ResortSplit>();
    }
}

namespace ThirdParty.Results.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// a list of search result
    /// </summary>
    /// <seealso cref="List{SearchResult}" />
    [XmlRoot("SearchResults")]
    public class SearchResults
    {
        /// <summary>
        /// Gets or sets the de-duplicates results.
        /// </summary>
        /// <value>
        /// The de-duplicates results.
        /// </value>
        public Dictionary<string, DedupeSearchResult> DedupeResults { get; set; } = new Dictionary<string, DedupeSearchResult>();

        /// <summary>
        /// Groups the results by property.
        /// </summary>
        /// <returns>a list of de-duplicated results</returns>
        public IEnumerable<DedupeSearchResult> GroupResultsByProperty()
        {
            return this.DedupeResults.Values
                .GroupBy(p => p.PropertyData.CentralPropertyID)
                .Select(g => new DedupeSearchResult()
                {
                    PropertyData = g.First().PropertyData,
                    RoomResults = g.SelectMany(v => v.RoomResults).ToList()
                });
        }
    }
}
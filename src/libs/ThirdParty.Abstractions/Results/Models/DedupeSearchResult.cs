namespace ThirdParty.Results.Models
{
    using System.Linq;

    /// <summary>
    /// A result that has been de-duplicated
    /// </summary>
    /// <seealso cref="ThirdParty.Results.Models.PropertySearchResult" />
    public class DedupeSearchResult : PropertySearchResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DedupeSearchResult"/> class.
        /// </summary>
        public DedupeSearchResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DedupeSearchResult"/> class.
        /// </summary>
        /// <param name="propertyResult">The property result.</param>
        public DedupeSearchResult(PropertySearchResult propertyResult)
        {
            this.PropertyData = propertyResult.PropertyData;
            this.RoomResults = propertyResult.RoomResults;
        }

        /// <summary>
        /// Gets the lead in price.
        /// </summary>
        /// <value>
        /// The lead in price.
        /// </value>
        public decimal LeadInPrice
        {
            get
            {
                return RoomResults
                    .GroupBy(r => r.RoomData.PropertyRoomBookingID)
                    .Select(r => r.Aggregate((r1, r2) => r1.Total < r2.Total ? r1 : r2))
                    .Sum(r => r.Total);
            }
        }
    }
}
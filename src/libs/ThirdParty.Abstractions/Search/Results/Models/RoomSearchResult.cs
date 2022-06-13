namespace ThirdParty.Results.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// a room search result
    /// </summary>
    public class RoomSearchResult
    {
        /// <summary>
        /// Gets or sets the result identifier.
        /// </summary>
        /// <value>
        /// The result identifier.
        /// </value>
        public int ResultID { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public decimal Total { get; set; }

        /// <summary>
        /// Gets or sets the total cost.
        /// </summary>
        /// <value>
        /// The total cost.
        /// </value>
        public decimal TotalCost { get; set; }

        /// <summary>Gets or sets a value indicating whether [non refundable].</summary>
        /// <value>
        /// <c>true</c> if [non refundable]; otherwise, <c>false</c>.</value>
        public bool NonRefundableRates { get; set; }

        /// <summary>
        /// Gets or sets the room data.
        /// </summary>
        /// <value>
        /// The room data.
        /// </value>
        public RoomData RoomData { get; set; } = new RoomData();

        /// <summary>
        /// Gets or sets the cancellations.
        /// </summary>
        /// <value>
        /// The cancellations.
        /// </value>
        public List<Cancellation> Cancellations { get; set; } = new List<Cancellation>();
    }
}
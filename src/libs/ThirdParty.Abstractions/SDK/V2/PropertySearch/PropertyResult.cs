namespace ThirdParty.SDK.V2.PropertySearch
{
    using System.Collections.Generic;

    /// <summary>
    /// A class representing a single property result
    /// </summary>
    public class PropertyResult
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>
        /// The booking token.
        /// </value>
        public string BookingToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the property identifier.
        /// </summary>
        /// <value>
        /// The property identifier.
        /// </value>
        public int PropertyID { get; set; }

        /// <summary>
        /// Gets or sets the room types.
        /// </summary>
        /// <value>
        /// The room types.
        /// </value>
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
    }
}
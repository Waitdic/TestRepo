namespace iVectorOne.SDK.V2.PropertySearch
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
        public string BookingToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the property identifier.
        /// </summary>
        public int PropertyID { get; set; }

        /// <summary>
        /// Gets or sets the room types.
        /// </summary>
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
    }
}
namespace ThirdParty.Results.Models
{
    /// <summary>
    /// Room Data
    /// </summary>
    public class RoomData
    {
        /// <summary>
        /// Gets or sets the result identifier.
        /// </summary>
        /// <value>
        /// The result identifier.
        /// </value>
        public int ResultID { get; set; }

        /// <summary>
        /// Gets or sets the meal basis.
        /// </summary>
        /// <value>
        /// The meal basis.
        /// </value>
        public string MealBasis { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the room.
        /// </summary>
        /// <value>
        /// The type of the room.
        /// </value>
        public string RoomType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the room view.
        /// </summary>
        /// <value>
        /// The room view.
        /// </value>
        public string RoomView { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the third party reference.
        /// </summary>
        /// <value>
        /// The third party reference.
        /// </value>
        public string TPReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the room type code.
        /// </summary>
        /// <value>
        /// The room type code.
        /// </value>
        public string RoomTypeCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        /// <value>
        /// The adults.
        /// </value>
        public int Adults { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets the children band2.
        /// </summary>
        /// <value>
        /// The children band2.
        /// </value>
        public int ChildrenBand2 { get; set; }

        /// <summary>
        /// Gets or sets the children band3.
        /// </summary>
        /// <value>
        /// The children band3.
        /// </value>
        public int ChildrenBand3 { get; set; }

        /// <summary>
        /// Gets or sets the youths.
        /// </summary>
        /// <value>
        /// The youths.
        /// </value>
        public int Youths { get; set; }

        /// <summary>
        /// Gets or sets the property room booking identifier.
        /// </summary>
        /// <value>
        /// The property room booking identifier.
        /// </value>
        public int PropertyRoomBookingID { get; set; }

        /// <summary>
        /// Gets or sets the property identifier.
        /// </summary>
        /// <value>
        /// The property identifier.
        /// </value>
        public int PropertyID { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether [non refundable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [non refundable]; otherwise, <c>false</c>.
        /// </value>
        public bool NonRefundable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pay local required].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [pay local required]; otherwise, <c>false</c>.
        /// </value>
        public bool PayLocalRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pay local available].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [pay local available]; otherwise, <c>false</c>.
        /// </value>
        public bool PayLocalAvailable { get; set; }
    }
}
namespace iVectorOne.SDK.V2.PropertyPrebook
{
    /// <summary>
    /// represents a room booking on a Property Pre book request
    /// </summary>
    public class RequestRoomBooking
    {
        /// <summary>Gets or sets the room booking token.</summary>
        public string RoomBookingToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference.</summary>
        public string SupplierReference1 { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference.</summary>
        public string SupplierReference2 { get; set; } = string.Empty;
    }
}
namespace iVectorOne.SDK.V2.PropertyPrebook
{
    using Newtonsoft.Json;

    /// <summary>
    /// represents a room booking on a Property Pre book request
    /// </summary>
    [JsonObject("RoomBooking")]
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
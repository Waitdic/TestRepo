namespace iVectorOne.SDK.V2.PropertyBook
{
    using iVectorOne.SDK.V2.Book;
    using System.Collections.Generic;

    /// <summary>
    /// represents a room booking on a Property Pre book request
    /// </summary>
    public class RoomBooking
    {
        /// <summary>Gets or sets the room booking token.</summary>
        public string RoomBookingToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference.</summary>
        public string SupplierReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the guest details.
        /// </summary>
        public List<GuestDetail> GuestDetails { get; set; } = new();

        /// <summary>
        /// Gets and sets the special request.
        /// </summary>
        public string SpecialRequest { get; set; } = string.Empty;
    }
}
namespace iVectorOne.SDK.V2.PropertyPrebook
{
    using System.Collections.Generic;
    using MediatR;

    public record Request : RequestBase, IRequest<Response>
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        public string BookingToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the room bookings.
        /// </summary>
        public List<RequestRoomBooking> RoomBookings { get; set; } = new();

        /// <summary>
        /// Get or sets the unique nationality identifier 
        /// </summary>
        public string NationalityID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the boolean to decide whether opaque rates are supported.
        /// </summary>
        public bool OpaqueRates { get; set; }

        /// <summary>
        /// Gets or sets the selling country.
        /// </summary>
        public string SellingCountry { get; set; } = string.Empty;
    }
}
namespace iVectorOne.SDK.V2.PropertyBook
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
        /// Gets or sets the Booking Reference.
        /// </summary>
        public string BookingReference { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference1.</summary>
        public string SupplierReference1 { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference2.</summary>
        public string SupplierReference2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the room bookings.
        /// </summary>
        public List<RoomBooking> RoomBookings { get; set; } = new();

        /// <summary>
        /// Gets or sets the lead customer.
        /// </summary>
        public LeadCustomer LeadCustomer { get; set; } = new();

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
﻿namespace iVectorOne.SDK.V2.TransferBook
{
    using System.Collections.Generic;
    using iVectorOne.SDK.V2.Book;
    using MediatR;

    public record Request : TransferRequestBase, IRequest<Response>
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        public string BookingToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Booking Reference.
        /// </summary>
        public string BookingReference { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference.</summary>
        public string SupplierReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the lead customer.
        /// </summary>
        public LeadCustomer LeadCustomer { get; set; } = new();

        /// <summary>
        /// Gets or sets the guest details.
        /// </summary>
        public List<GuestDetail> GuestDetails { get; set; } = new();

        /// <summary>
        /// Gets or sets the outbound pickup details.
        /// </summary>
        public JourneyDetails OutboundPickUpDetails { get; set; } = new();

        /// <summary>
        /// Gets or sets the outbound drop off details.
        /// </summary>
        public JourneyDetails OutboundDropOffDetails { get; set; } = new();

        /// <summary>
        /// Gets or sets the return pickup details.
        /// </summary>
        public JourneyDetails ReturnPickUpDetails { get; set; } = new();

        /// <summary>
        /// Gets or sets the return drop off details.
        /// </summary>
        public JourneyDetails ReturnDropOffDetails { get; set; } = new();

        /// <summary>
        /// Get or sets the unique nationality identifier 
        /// </summary>
        ///public string NationalityID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the selling country.
        /// </summary>
        ///public string SellingCountry { get; set; } = string.Empty;
    }
}
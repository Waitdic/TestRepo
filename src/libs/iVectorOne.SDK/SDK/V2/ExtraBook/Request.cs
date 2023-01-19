namespace iVectorOne.SDK.V2.ExtraBook
{
    using System.Collections.Generic;
    using iVectorOne.SDK.V2.Book;
    using MediatR;

    public record Request : ComponentRequestBase, IRequest<Response>
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
    }
}
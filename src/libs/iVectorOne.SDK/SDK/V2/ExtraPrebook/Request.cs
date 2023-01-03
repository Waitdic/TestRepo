namespace iVectorOne.SDK.V2.ExtraPrebook
{
    using System.Collections.Generic;
    using MediatR;

    public record Request : TransferRequestBase, IRequest<Response>
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        public string BookingToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference.</summary>
        public string SupplierReference { get; set; } = string.Empty;

        /// <summary>
        /// Get or sets the unique nationality identifier 
        /// </summary>
        //public string NationalityID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the selling country.
        /// </summary>
        //public string SellingCountry { get; set; } = string.Empty;
    }
}
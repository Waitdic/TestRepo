namespace iVectorOne.SDK.V2.TransferPrebook
{
    using MediatR;

    public record Request : ComponentRequestBase, IRequest<Response>
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        public string BookingToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference.</summary>
        public string SupplierReference { get; set; } = string.Empty;
    }
}
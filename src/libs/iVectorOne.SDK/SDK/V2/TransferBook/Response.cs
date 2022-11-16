namespace iVectorOne.SDK.V2.TransferBook
{
    public record Response : ResponseBase
    {
        /// <summary>Gets or sets the supplier booking reference.</summary>
        public string SupplierBookingReference { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference.</summary>
        public string SupplierReference { get; set; } = string.Empty;
    }
}
namespace iVectorOne.SDK.V2.PropertyCancel
{
    public record Response : ResponseBase
    {
        /// <summary>
        /// Gets or sets the supplier cancellation reference.
        /// </summary>
        public string SupplierCancellationReference { get; set; } = string.Empty;
    }
}
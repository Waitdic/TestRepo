namespace iVectorOne.SDK.V2.PropertyPrecancel
{
    public record Response : ResponseBase
    {
        /// <summary>
        /// Gets or sets the supplier cancellation reference.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        ///  Gets or sets The currency code
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
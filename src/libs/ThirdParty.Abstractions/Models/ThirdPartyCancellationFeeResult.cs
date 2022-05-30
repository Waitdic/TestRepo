namespace ThirdParty.Models
{
    /// <summary>
    /// Third Party Cancellation Fee Result
    /// </summary>
    public class ThirdPartyCancellationFeeResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether it was successful
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        ///  Gets or sets The amount
        /// </summary>
        public decimal Amount { get; set; } = 0;

        /// <summary>
        ///  Gets or sets The currency code
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;
    }
}

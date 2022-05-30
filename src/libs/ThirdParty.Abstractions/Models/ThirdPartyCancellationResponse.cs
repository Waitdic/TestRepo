namespace ThirdParty.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Third Party Cancellation Response
    /// </summary>
    public class ThirdPartyCancellationResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether there was success
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// Gets or sets The amount
        /// </summary>
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// Gets or sets The currency code
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The third party cancellation reference
        /// </summary>
        public string TPCancellationReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether cost received from third party
        /// </summary>
        public bool CostRecievedFromThirdParty { get; set; } = false;

        /// <summary>
        /// Gets or sets The logs
        /// </summary>
        public List<Log> Logs { get; set; } = new List<Log>();
    }
}
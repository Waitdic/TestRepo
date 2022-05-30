namespace ThirdParty.SDK.V2.PropertyPrecancel
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public record Response
    {
        /// <summary>
        /// Gets or sets the supplier cancellation reference.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        ///  Gets or sets The currency code
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>Any warnings raised on the cancellation response</summary>
        /// <value>The warnings.</value>
        [JsonIgnore]
        public List<string> Warnings { get; set; } = new();
    }
}
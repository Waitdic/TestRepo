namespace ThirdParty.SDK.V2.PropertyCancel
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public record Response
    {
        /// <summary>
        /// Gets or sets the supplier cancellation reference.
        /// </summary>
        public string SupplierCancellationReference { get; set; } = string.Empty;

        /// <summary>Any warnings raised on the cancellation response</summary>
        /// <value>The warnings.</value>
        [JsonIgnore]
        public List<string> Warnings { get; set; } = new();
    }
}
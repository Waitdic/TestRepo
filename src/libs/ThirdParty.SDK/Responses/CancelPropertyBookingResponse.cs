namespace ThirdParty.SDK.Responses
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// A response returned when a property is cancelled.
    /// </summary>
    public class CancelPropertyBookingResponse
    {
        /// <summary>
        /// Gets or sets the supplier cancellation reference.
        /// </summary>
        /// <value>
        /// The supplier cancellation reference.
        /// </value>
        public string SupplierCancellationReference { get; set; } = string.Empty;

        /// <summary>Any warnings raised on the cancellation response</summary>
        /// <value>The warnings.</value>
        [JsonIgnore]
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
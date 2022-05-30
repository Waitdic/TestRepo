namespace ThirdParty.SDK.V2.PropertyBook
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public record Response
    {
        /// <summary>Gets or sets the supplier booking reference.</summary>
        public string SupplierBookingReference { get; set; } = string.Empty;

        /// <summary>
        ///   <para>Token to be passed into the cancel request</para>
        /// </summary>
        public string BookToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference1.</summary>
        public string SupplierReference1 { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference1.</summary>
        public string SupplierReference2 { get; set; } = string.Empty;

        /// <summary>Any warnings raised on the book response</summary>
        [JsonIgnore]
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
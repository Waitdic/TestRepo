namespace ThirdParty.Models.Tokens
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>A class that represents the property token, will be encrypted in responses/requests</summary>
    public class PropertyToken
    {
        /// <summary>Gets or sets the Central Property Identifier</summary>
        [JsonPropertyName("CP")]
        public int CentralPropertyID { get; set; }

        /// <summary>Gets or sets the arrival date</summary>
        [JsonPropertyName("A")]
        public DateTime ArrivalDate { get; set; }

        /// <summary>Gets or sets the duration</summary>
        [JsonPropertyName("D")]
        public int Duration { get; set; }

        /// <summary>Gets or sets the source</summary>
        [JsonPropertyName("S")]
        public string Source { get; set; } = string.Empty;

        /// <summary>Gets or sets the source</summary>
        [JsonPropertyName("N")]
        public int Rooms { get; set; }

        /// <summary>Gets or sets the Third Party key</summary>
        [JsonPropertyName("TK")]
        public string TPKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the Property Identifier</summary>
        [JsonPropertyName("P")]
        public int PropertyID { get; set; }

        /// <summary>Gets or sets the currency id</summary>
        [JsonPropertyName("CI")]
        public int CurrencyID { get; set; }

        /// <summary>Gets or sets the property name.</summary>
        [JsonPropertyName("PN")]
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>Gets or sets the geography code</summary>
        [JsonPropertyName("GC")]
        public string GeographyCode { get; set; } = string.Empty;
    }
}
namespace ThirdParty.Models.Tokens
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>A class that represents the property token, will be encrypted in responses/requests</summary>
    public class PropertyToken
    {
        /// <summary>
        ///   <para>
        /// Gets or sets the Central Property Identifier</para>
        /// </summary>
        /// <value>The Central Property Identifier.</value>
        [JsonPropertyName("CP")]
        public int CentralPropertyID { get; set; }

        /// <summary>Gets or sets the arrival date</summary>
        /// <value>The arrival date.</value>
        [JsonPropertyName("A")]
        public DateTime ArrivalDate { get; set; }

        /// <summary>Gets or sets the duration</summary>
        /// <value>The duration</value>
        [JsonPropertyName("D")]
        public int Duration { get; set; }

        /// <summary>Gets or sets the source</summary>
        /// <value>The source.</value>
        [JsonPropertyName("S")]
        public string Source { get; set; } = string.Empty;

        /// <summary>Gets or sets the source</summary>
        /// <value>The source.</value>
        [JsonPropertyName("N")]
        public int Rooms { get; set; }

        /// <summary>Gets or sets the Third Party key</summary>
        /// <value>Third Party key</value>
        [JsonPropertyName("TK")]
        public string TPKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the Property Identifier</summary>
        /// <value>Property Identifier</value>
        [JsonPropertyName("P")]
        public int PropertyID { get; set; }

        /// <summary>Gets or sets the currency id</summary>
        /// <value>The iso currency id</value>
        [JsonPropertyName("CI")]
        public int CurrencyID { get; set; }
    }
}
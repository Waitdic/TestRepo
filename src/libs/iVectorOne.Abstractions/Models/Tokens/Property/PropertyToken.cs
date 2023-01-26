namespace iVectorOne.Models.Tokens
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>A class that represents the property token, will be encrypted in responses/requests</summary>
    public class PropertyToken
    {
        /// <summary>Gets or sets the arrival date</summary>
        [JsonPropertyName("A")]
        public DateTime ArrivalDate { get; set; }

        /// <summary>Gets or sets the duration</summary>
        [JsonPropertyName("D")]
        public int Duration { get; set; }

        /// <summary>Gets or sets the source</summary>
        [JsonPropertyName("N")]
        public int Rooms { get; set; }

        /// <summary>Gets or sets the Property Identifier</summary>
        [JsonPropertyName("P")]
        public int PropertyID { get; set; }

        /// <summary>Gets or sets the currency id</summary>
        [JsonPropertyName("CI")]
        public int ISOCurrencyID { get; set; }
    }
}
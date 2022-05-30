namespace ThirdParty.Models.Tokens
{
    using System;
    using Newtonsoft.Json;

    /// <summary>A class that represents the property token, will be encrypted in responses/requests</summary>
    public class PropertyToken
    {
        /// <summary>
        ///   <para>
        /// Gets or sets the Central Property Identifier</para>
        /// </summary>
        /// <value>The Central Property Identifier.</value>
        [JsonProperty("CP")]
        public int CentralPropertyID { get; set; }

        /// <summary>Gets or sets the arrival date</summary>
        /// <value>The arrival date.</value>
        [JsonProperty("A")]
        public DateTime ArrivalDate { get; set; }

        /// <summary>Gets or sets the duration</summary>
        /// <value>The duration</value>
        [JsonProperty("D")]
        public int Duration { get; set; }

        /// <summary>Gets or sets the source</summary>
        /// <value>The source.</value>
        [JsonProperty("S")]
        public string Source { get; set; } = string.Empty;

        /// <summary>Gets or sets the source</summary>
        /// <value>The source.</value>
        [JsonProperty("N")]
        public int Rooms { get; set; }

        /// <summary>Gets or sets the Third Party key</summary>
        /// <value>Third Party key</value>
        [JsonProperty("TK")]
        public string TPKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the Property Identifier</summary>
        /// <value>Property Identifier</value>
        [JsonProperty("P")]
        public int PropertyID { get; set; }

        /// <summary>Gets or sets the currency id</summary>
        /// <value>The iso currency id</value>
        [JsonProperty("CI")]
        public int CurrencyID { get; set; }
    }
}

namespace iVectorOne.Models.Tokens
{
    using System;
    using Newtonsoft.Json;

    /// <summary>A class that represents the transfer token, will be encrypted in responses/requests</summary>
    public class TransferToken
    {
        /// <summary>Gets or sets the departure date</summary>
        /// <value>The departure date.</value>
        [JsonProperty("D")]
        public DateTime DepartureDate { get; set; }

        /// <summary>Gets or sets the source</summary>
        /// <value>The source.</value>
        [JsonProperty("S")]
        public string Source { get; set; } = string.Empty;

        /// <summary>Gets or sets the currency id</summary>
        /// <value>The iso currency id</value>
        [JsonProperty("CI")]
        public int CurrencyID { get; set; }
    }
}

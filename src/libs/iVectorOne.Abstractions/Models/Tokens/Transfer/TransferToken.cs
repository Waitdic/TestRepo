namespace iVectorOne.Models.Tokens
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
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

        /// <summary>Gets or sets the Third Party Session ID</summary>
        [JsonPropertyName("SID")]
        public string TPSessionID { get; set; } = string.Empty;

        /// <summary>Gets or sets the currency id</summary>
        [JsonPropertyName("CI")]
        public int ISOCurrencyID { get; set; }

        /// <summary>Gets or sets the number of Adults</summary>
        [JsonPropertyName("A")]
        public int Adults { get; set; }

        /// <summary>Gets or sets the number of children</summary>
        [JsonPropertyName("C")]
        public int Children { get; set; }

        /// <summary>Gets or sets the number of infants</summary>
        [JsonPropertyName("I")]
        public int Infants { get; set; }

        /// <summary>Gets or sets the child ages</summary>
        //[JsonPropertyName("CA")]
        //public List<int> ChildAges { get; set; } = new();

        /// <summary>Gets or sets the Local cost</summary>
        [JsonPropertyName("AMT")]
        public List<int> LocalCost { get; set; } = new();
    }
}

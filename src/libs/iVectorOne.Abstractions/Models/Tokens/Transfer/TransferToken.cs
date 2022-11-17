namespace iVectorOne.Models.Tokens.Transfer
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json;

    /// <summary>A class that represents the transfer token, will be encrypted in responses/requests</summary>
    public class TransferToken
    {
        /// <summary>Gets or sets the source</summary>
        [JsonPropertyName("S")]
        public string Source { get; set; } = string.Empty;

        /// <summary>Gets or sets the departure date</summary>
        /// <value>The departure date.</value>
        [JsonProperty("D")]
        public DateTime DepartureDate { get; set; }

        ///// <summary>Gets or sets the departure time</summary>
        ///// <value>The departure time.</value>
        //[JsonProperty("DT")]
        //public string DepartureTime { get; set; } = string.Empty;

        //[JsonPropertyName("D")]
        //public int Duration { get; set; }

        ///// <summary>Gets or sets the return time</summary>
        ///// <value>The return time.</value>
        //[JsonProperty("RT")]
        //public string ReturnTime { get; set; } = string.Empty;

        /// <summary>Gets or sets the departure location id</summary>
        //[JsonPropertyName("DL")]
        //public int DepartureLocationID { get; set; }

        /// <summary>Gets or sets the arrival location id</summary>
        //[JsonPropertyName("A")]
        //public int ArrivalLocationID { get; set; }

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
        //[JsonPropertyName("AMT")]
        //public List<int> LocalCost { get; set; } = new();

        /// <summary>Gets or sets the supplier id</summary>
        [JsonPropertyName("SI")]
        public int SupplierID { get; set; }

        [JsonPropertyName("TB")]
        public int TransferBookingID { get; set; }
    }
}

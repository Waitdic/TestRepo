﻿namespace ThirdParty.Models.Tokens
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    ///   <para>Represents a room token used to compress details between search and book / pre book</para>
    /// </summary>
    public class RoomToken
    {
        /// <summary>Gets or sets the number of Adults</summary>
        /// <value>the number of Adults in the room</value>
        [JsonProperty("A")]
        public int Adults { get; set; }

        /// <summary>Gets or sets the number of children</summary>
        /// <value>The number of children in the room</value>
        [JsonProperty("C")]
        public int Children { get; set; }

        /// <summary>Gets or sets the number of infants</summary>
        /// <value>The number of infants in the room</value>
        [JsonProperty("I")]
        public int Infants { get; set; }

        /// <summary>Gets or sets the child ages</summary>
        /// <value>The number of the child ages</value>
        [JsonProperty("CA")]
        public List<int> ChildAges { get; set; } = new List<int>();

        /// <summary>Gets or sets the Local cost</summary>
        /// <value>The Local cost</value>
        [JsonProperty("AMT")]
        public List<int> LocalCost { get; set; } = new List<int>();

        /// <summary>Gets or sets the property room booking id</summary>
        /// <value>The property room booking id</value>
        [JsonProperty("PRI")]
        public int PropertyRoomBookingID { get; set; }

        /// <summary>Gets or sets the mealbasis</summary>
        /// <value>The meal basis</value>
        [JsonProperty("MBC")]
        public List<int> MealBasis { get; set; } = new List<int>();
    }
}
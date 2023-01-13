namespace iVectorOne.Models.Tokens
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    ///   <para>Represents a room token used to compress details between search and book / pre book</para>
    /// </summary>
    public class RoomToken
    {
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
        [JsonPropertyName("CA")]
        public List<int> ChildAges { get; set; } = new();

        /// <summary>Gets or sets the Local cost</summary>
        [JsonPropertyName("AMT")]
        public decimal LocalCost { get; set; } = new();

        /// <summary>Gets or sets the property room booking id</summary>
        [JsonPropertyName("PRI")]
        public int PropertyRoomBookingID { get; set; }

        /// <summary>Gets or sets the mealbasis</summary>
        [JsonPropertyName("MBC")]
        public int MealBasisID { get; set; } = new();

        /// <summary>Gets or sets the property identifier</summary>
        [JsonPropertyName("P")]
        public int PropertyID { get; set; }
    }
}
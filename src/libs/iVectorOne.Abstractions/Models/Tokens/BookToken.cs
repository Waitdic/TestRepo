namespace iVectorOne.Models.Tokens
{
    using System.Text.Json.Serialization;

    /// <summary>A class that represents the book token, will be encrypted in responses/requests,
    /// returned from the book response and used in the cancel request</summary>
    public class BookToken
    {
        /// <summary>Gets or sets the property identifier.</summary>
        [JsonPropertyName("P")]
        public int PropertyID { get; set; }

        /// <summary>Gets or sets the Source.</summary>
        [JsonPropertyName("S")]
        public string Source { get; set; } = string.Empty;
    }
}
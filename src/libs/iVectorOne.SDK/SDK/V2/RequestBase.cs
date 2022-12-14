namespace iVectorOne.SDK.V2
{
    using iVectorOne.Models;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public record RequestBase
    {
        [JsonIgnore]
        public Account Account { get; set; } = new();

        [JsonIgnore]
        public int BookingID { get; set; }

        public Dictionary<string, string> ThirdPartySettings { get; set; } = new Dictionary<string, string> { };
    }
}
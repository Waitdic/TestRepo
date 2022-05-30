namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class PropertyAvailablility
    {

        [JsonProperty("property_id")]
        public string PropertyID { get; set; }

        [JsonProperty("rooms")]
        public List<SearchResponseRoom> Rooms { get; set; } = new List<SearchResponseRoom>();

        [JsonProperty("links")]
        public Dictionary<string, Link> AvailabilityLinks { get; set; } = new Dictionary<string, Link>();

        [JsonProperty("score")]
        public int Score { get; set; }

    }
}
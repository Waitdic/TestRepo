namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class RoomInfo
    {
        [JsonProperty("configuration")]
        public string Configuration { get; set; } = string.Empty;

        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("pricing")]
        public Pricing Pricing { get; set; } = new();
    }
}

namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class CancellationPolicy
    {
        [JsonProperty("from")]
        public string From { get; set; } = string.Empty;

        [JsonProperty("to")]
        public string To { get; set; } = string.Empty;

        [JsonProperty("pricing")]
        public Pricing Pricing { get; set; } = new();
    }
}

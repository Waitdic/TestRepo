namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class Pricing
    {
        [JsonProperty("commision")]
        public Commision Commision { get; set; } = new();

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonProperty("net")]
        public Amount Net { get; set; } = new();

        [JsonProperty("sell")]
        public Amount Sell { get; set; } = new();
    }
}

namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class RoomRate
    {
        [JsonProperty("rateName")]
        public string RateName { get; set; } = string.Empty;

        [JsonProperty("rateId")]
        public string RateId { get; set; } = string.Empty;

        [JsonProperty("roomQty")]
        public int RoomQty { get; set; }

        [JsonProperty("rooms")]
        public List<RoomInfo> Rooms { set; get; } = new();

        [JsonProperty("pricing")]
        public Pricing Pricing { get; set; } = new();

        [JsonProperty("meal")]
        public Meal Meal { get; set; } = new();

        [JsonProperty("cancellationPolicies")]
        public List<CancellationPolicy> CancellationPolicies { get; set; } = new();

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("bookToken")]
        public string BookToken { get; set; } = string.Empty;
    }
}

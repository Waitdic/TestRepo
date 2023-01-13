namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;
    public class HotelAvailabilityRequest
    {
        [JsonProperty("searchAvail")]
        public SearchAvail SearchAvail { get; set; } = new();

        [JsonProperty("timeout")]
        public int Timeout { get; set; }
        public bool ShouldSerializeTimeout() => Timeout != 0;
    }
}

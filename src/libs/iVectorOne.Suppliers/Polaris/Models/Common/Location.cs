using Newtonsoft.Json;

namespace iVectorOne.Suppliers.Polaris.Models
{
    public class Location
    {
        [JsonProperty("destinationCode")]
        public string DestinationCode { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = Constant.LocationType.Empty;
    }

}

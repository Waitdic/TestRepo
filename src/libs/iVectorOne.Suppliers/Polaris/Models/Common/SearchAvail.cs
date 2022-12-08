namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class SearchAvail
    {
        [JsonProperty("checkIn")]
        public string CheckIn { get; set; } = string.Empty;

        [JsonProperty("checkOut")]
        public string CheckOut { get; set; } = string.Empty;

        [JsonProperty("destination")]
        public Destination Destination { get; set; } = new();

        [JsonProperty("rooms")]
        public List<RoomRequest> Rooms { get; set; } = new();

        [JsonProperty("market")]
        public string Market { get; set; } = string.Empty;
        public bool ShouldSerializeMarket() => !string.IsNullOrEmpty(Market);
    }
}

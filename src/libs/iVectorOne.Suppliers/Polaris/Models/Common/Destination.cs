namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class Destination
    {
        [JsonProperty("hotelCodes")]
        public List<string> HotelCodes { get; set; } = new();
        public bool ShouldSerializeHotelCodes() => HotelCodes.Any();

        [JsonProperty("location")]
        public Location Location { get; set; } = new();
        public bool ShouldSerializeLocation() => !string.IsNullOrEmpty($"{Location.DestinationCode}");
    }
}

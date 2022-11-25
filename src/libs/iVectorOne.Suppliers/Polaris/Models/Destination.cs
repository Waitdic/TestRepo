namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class Destination 
    {
        [JsonPropertyName("destinationCode")]
        public List<string> HotelCodes { get; set; } = new();

        [JsonPropertyName("type")]
        public Location Location { get; set; } = new();
    }

}

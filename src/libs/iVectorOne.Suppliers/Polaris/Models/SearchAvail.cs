namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class SearchAvail 
    {
        [JsonPropertyName("checkIn")]
        public string CheckIn { get; set; } = string.Empty;

        [JsonPropertyName("checkOut")]
        public string CheckOut { get; set; } = string.Empty;

        [JsonPropertyName("destination")]
        public Destination Destination { get; set; } = new();

        [JsonPropertyName("rooms")]
        public List<RoomRequest> Rooms { get; set; } = new();
    }

}

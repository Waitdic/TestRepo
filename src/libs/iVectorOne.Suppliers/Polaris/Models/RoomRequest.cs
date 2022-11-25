namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class RoomRequest 
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("passengerAges")]
        public List<int> PassengerAges { get; set; } = new();
    }

}

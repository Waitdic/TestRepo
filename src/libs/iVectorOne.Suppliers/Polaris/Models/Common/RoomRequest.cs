namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class RoomRequest
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("passengerAges")]
        public List<int> PassengerAges { get; set; } = new();
    }

}

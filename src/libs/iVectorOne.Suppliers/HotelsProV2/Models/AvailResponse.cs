namespace iVectorOne.CSSuppliers.HotelsProV2.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class AvailResponse
    {
        [JsonProperty("rooms")]
        public List<Room> Rooms { get; set; } = new();

        [JsonProperty("code")]
        public string RoomCode { get; set; } = string.Empty;

        [JsonProperty("price")]
        public string Price { get; set; } = string.Empty;
    }
}

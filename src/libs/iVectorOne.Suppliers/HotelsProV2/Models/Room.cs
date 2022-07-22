namespace iVectorOne.CSSuppliers.HotelsProV2.Models
{
    using Newtonsoft.Json;

    public class Room
    {
        [JsonProperty("pax")]
        public Pax Pax { get; set; } = new();

        [JsonProperty("room_type")]
        public string RoomTypeCode { get; set; } = string.Empty;

        [JsonProperty("room_description")]
        public string RoomType { get; set; } = string.Empty;
    }
}

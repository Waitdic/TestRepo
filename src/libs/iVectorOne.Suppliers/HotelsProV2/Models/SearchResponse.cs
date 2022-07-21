namespace iVectorOne.CSSuppliers.HotelsProV2.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class SearchResponse
    {
        [JsonProperty("room_code")]
        public string RoomCode { get; set; } = string.Empty;

        [JsonProperty("hotel_code")]
        public string HotelCode { get; set; } = string.Empty;

        [JsonProperty("products")]
        public List<Product> Products { get; set; } = new();
    }
}

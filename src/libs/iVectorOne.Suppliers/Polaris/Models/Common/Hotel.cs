namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Hotel
    {
        [JsonProperty("chekIn")]
        public string CheckIn { get; set; } = string.Empty;

        [JsonProperty("chekOut")]
        public string CheckOut { get; set; } = string.Empty;

        [JsonProperty("hotelCode")]
        public string HotelCode { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("roomRates")]
        public List<RoomRate> RoomRates { get; set; } = new();
    }

}

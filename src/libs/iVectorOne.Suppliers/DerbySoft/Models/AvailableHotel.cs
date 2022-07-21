namespace iVectorOne.Suppliers.DerbySoft.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class AvailableHotel
    {
        [JsonProperty("stayRange")]
        public StayRange StayRange { get; set; }

        [JsonProperty("availRoomRates")]
        public RoomRate[] AvailableRoomRates { get; set; }

        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }

        [JsonProperty("hotelId")]
        public string HotelId { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public HotelStatus? Status { get; set; }
    }
}
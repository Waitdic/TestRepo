namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class AvailRequest 
    {
        [JsonProperty("hotelAvailability")]
        public HotelAvailabilityRequest HotelAvailability { get; set; } = new();

        [JsonProperty("token")]
        public string Token { get; set; }

        public bool ShouldSerializeToken() => !string.IsNullOrEmpty(Token);
    }

}

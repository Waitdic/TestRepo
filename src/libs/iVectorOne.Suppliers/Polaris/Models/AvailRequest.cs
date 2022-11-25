namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Text.Json.Serialization;

    public class AvailRequest 
    {
        [JsonPropertyName("hotelAvailability")]
        public HotelAvailabilityRequest HotelAvailability { get; set; } = new();

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }

}

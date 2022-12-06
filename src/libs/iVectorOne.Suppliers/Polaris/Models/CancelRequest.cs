namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class CancelRequest 
    {
        [JsonProperty("bookingReference")]
        public BookingReference BookingReference { get; set; } = new();

        [JsonProperty("token")]
        public string Token { get; set; } = string.Empty;
        public bool ShouldSerializeToken() => !string.IsNullOrEmpty(Token);
    }
}

namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class BookRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; } = string.Empty;
        public bool ShouldSerializeToken() => !string.IsNullOrEmpty(Token);

        [JsonProperty("bookToken")]
        public string BookToken { get; set; } = string.Empty;

        [JsonProperty("bookingReference")]
        public BookingReference BookingReference { get; set; } = new();

        [JsonProperty("remarks")]
        public string Remarks { get; set; } = string.Empty;

        [JsonProperty("travellers")]
        public List<Traveller> Travellers { get; set; } = new();
    }
}

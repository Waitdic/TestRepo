namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class DetailResponse 
    {
        [JsonProperty("hotel")]
        public Hotel Hotel { get; set; } = new();

        [JsonProperty("token")]
        public string Token { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
    }
}

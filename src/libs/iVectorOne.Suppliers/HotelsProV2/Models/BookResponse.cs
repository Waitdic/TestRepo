namespace iVectorOne.Suppliers.HotelsProV2.Models
{
    using Newtonsoft.Json;

    public class BookResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;
    }
}

namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class PrebookRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; } = string.Empty;
        public bool ShouldSerializeToken() => !string.IsNullOrEmpty(Token);

        [JsonProperty("bookToken")]
        public string BookToken { get; set; } = string.Empty;
    }
}

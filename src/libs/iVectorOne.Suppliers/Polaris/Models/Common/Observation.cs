namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class Observation
    {
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("txt")]
        public string Txt { get; set; } = string.Empty;
    }
}

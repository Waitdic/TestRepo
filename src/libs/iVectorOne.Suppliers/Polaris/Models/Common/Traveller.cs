namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class Traveller
    {
        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("person")]
        public Person Person { get; set; } = new();
    }
}

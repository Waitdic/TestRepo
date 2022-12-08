namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class Person
    {
        [JsonProperty("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}

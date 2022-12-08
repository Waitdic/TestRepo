namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class Amount
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}

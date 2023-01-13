namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class Commision
    {
        [JsonProperty("comAgency")]
        public Amount ComAgency { get; set; } = new();

        [JsonProperty("comTaxAmount")]
        public decimal ComTaxAmount { get; set; }

        [JsonProperty("comTaxPercent")]
        public decimal ComTaxPercent { get; set; }
    }
}
